using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Areas.Member.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
public class ManagementDuesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ManagementDuesController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var boardMember = await GetCurrentBoardMemberAsync(identityUserId);

        if (boardMember == null)
        {
            return View(new ManagementDuesViewModel());
        }

        var dues = await _context.MemberDues
            .AsNoTracking()
            .Where(due =>
                due.BoardMemberId == boardMember.Id &&
                due.Status != MemberDueStatuses.Cancelled)
            .Include(due => due.DuesPeriod)
            .Include(due => due.Payments)
            .OrderByDescending(due => due.DuesPeriod.StartDate)
            .ToListAsync();

        var today = DateTime.Today;
        var items = dues.Select(due =>
        {
            var payment = due.Payments
                .Where(item =>
                    item.Status == DuesPaymentStatuses.Completed ||
                    item.Status == DuesPaymentStatuses.PartiallyRefunded ||
                    item.Status == DuesPaymentStatuses.Refunded)
                .OrderByDescending(item => item.PaidAtUtc)
                .FirstOrDefault();

            return new MemberDueItemViewModel
            {
                Id = due.Id,
                PaymentId = payment?.Id,
                PeriodName = due.DuesPeriod.Name,
                PeriodType = due.DuesPeriod.PeriodType,
                Description = due.DuesPeriod.Description,
                Amount = due.Amount,
                StartDate = due.DuesPeriod.StartDate,
                EndDate = due.DuesPeriod.EndDate,
                DueDate = due.DuesPeriod.DueDate,
                Status = due.Status,
                IsOverdue = due.Status == MemberDueStatuses.Unpaid &&
                    due.DuesPeriod.DueDate.Date < today,
                PaidAtUtc = due.PaidAtUtc,
                PaymentMethod = payment?.PaymentMethod,
                ReferenceNumber = payment?.ReferenceNumber
            };
        }).ToList();

        var validDues = dues
            .Where(due => due.Status != MemberDueStatuses.Exempt)
            .ToList();

        return View(new ManagementDuesViewModel
        {
            HasLinkedBoardMembership = true,
            BoardMemberName = boardMember.FullName,
            Dues = new MemberDuesViewModel
            {
                OutstandingAmount = validDues
                    .Where(due => due.Status == MemberDueStatuses.Unpaid)
                    .Sum(due => due.Amount),
                PaidAmount = validDues
                    .Where(due => due.Status == MemberDueStatuses.Paid)
                    .Sum(due => due.Amount),
                UnpaidCount = validDues.Count(due =>
                    due.Status == MemberDueStatuses.Unpaid),
                OverdueCount = items.Count(item => item.IsOverdue),
                Items = items
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var payments = await _context.DuesPayments
            .AsNoTracking()
            .Where(payment =>
                payment.MemberDue.BoardMember != null &&
                payment.MemberDue.BoardMember.IdentityUserId == identityUserId &&
                payment.MemberDue.BoardMember.IsActive)
            .OrderByDescending(payment => payment.PaidAtUtc)
            .Select(payment => new MemberPaymentHistoryItemViewModel
            {
                Id = payment.Id,
                PeriodName = payment.MemberDue.DuesPeriod.Name,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Provider = payment.Provider,
                ReferenceNumber = payment.ReferenceNumber,
                PaidAtUtc = payment.PaidAtUtc,
                RefundedAmount = payment.RefundedAmount,
                IsDemo =
                    payment.Provider == "PayTR Demo" &&
                    payment.ReferenceNumber != null &&
                    payment.ReferenceNumber.StartsWith("DEMO-"),
                CanViewReceipt =
                    payment.Status == DuesPaymentStatuses.Completed ||
                    payment.Status == DuesPaymentStatuses.PartiallyRefunded ||
                    payment.Status == DuesPaymentStatuses.Refunded
            })
            .ToListAsync();

        return View(new MemberPaymentHistoryViewModel
        {
            PaymentCount = payments.Count,
            NetPaidAmount = payments
                .Where(payment =>
                    payment.Status == DuesPaymentStatuses.Completed ||
                    payment.Status == DuesPaymentStatuses.PartiallyRefunded)
                .Sum(payment => Math.Max(
                    0,
                    payment.Amount - payment.RefundedAmount)),
            RefundedAmount = payments.Sum(payment => payment.RefundedAmount),
            Payments = payments
        });
    }

    [HttpGet]
    public async Task<IActionResult> PaymentReceipt(int id)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var payment = await _context.DuesPayments
            .AsNoTracking()
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.BoardMember)
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.DuesPeriod)
            .SingleOrDefaultAsync(item =>
                item.Id == id &&
                item.MemberDue.BoardMember != null &&
                item.MemberDue.BoardMember.IdentityUserId == identityUserId &&
                item.MemberDue.BoardMember.IsActive);

        if (payment == null)
        {
            return NotFound();
        }

        var isSuccessfulPayment =
            payment.Status == DuesPaymentStatuses.Completed ||
            payment.Status == DuesPaymentStatuses.PartiallyRefunded ||
            payment.Status == DuesPaymentStatuses.Refunded;

        if (!isSuccessfulPayment)
        {
            TempData["ErrorMessage"] =
                "Yalnızca tamamlanmış ödeme işlemleri için ödeme belgesi görüntülenebilir.";
            return RedirectToAction(nameof(Index));
        }

        return View(new DuesPaymentReceiptViewModel
        {
            PaymentId = payment.Id,
            DuesPeriodId = payment.MemberDue.DuesPeriodId,
            DocumentNumber = string.IsNullOrWhiteSpace(payment.ReferenceNumber)
                ? $"ODEME-{payment.Id:D6}"
                : payment.ReferenceNumber,
            MemberName = payment.MemberDue.BoardMember!.FullName,
            RegistryNumber = null,
            PeriodName = payment.MemberDue.DuesPeriod.Name,
            PeriodType = payment.MemberDue.DuesPeriod.PeriodType,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            Provider = payment.Provider,
            ReferenceNumber = payment.ReferenceNumber,
            PaidAtUtc = payment.PaidAtUtc,
            IsDemo =
                payment.Provider == "PayTR Demo" &&
                payment.ReferenceNumber?.StartsWith(
                    "DEMO-",
                    StringComparison.OrdinalIgnoreCase) == true
        });
    }

    [HttpGet]
    public async Task<IActionResult> DemoCheckout(int dueId)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var due = await _context.MemberDues
            .AsNoTracking()
            .Include(item => item.DuesPeriod)
            .Include(item => item.BoardMember)
            .SingleOrDefaultAsync(item =>
                item.Id == dueId &&
                item.BoardMember != null &&
                item.BoardMember.IdentityUserId == identityUserId &&
                item.BoardMember.IsActive);

        if (due == null)
        {
            return NotFound();
        }

        if (due.Status != MemberDueStatuses.Unpaid)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(new DemoPaymentViewModel
        {
            MemberDueId = due.Id,
            MemberName = due.BoardMember!.FullName,
            PeriodName = due.DuesPeriod.Name,
            Amount = due.Amount,
            DueDate = due.DuesPeriod.DueDate,
            IsOverdue = due.DuesPeriod.DueDate.Date < DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SimulateDemoPayment(
        int memberDueId,
        string outcome)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        if (outcome != "success" && outcome != "failure")
        {
            return BadRequest();
        }

        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var due = await _context.MemberDues
            .Include(item => item.BoardMember)
            .Include(item => item.Payments)
            .SingleOrDefaultAsync(item =>
                item.Id == memberDueId &&
                item.BoardMember != null &&
                item.BoardMember.IdentityUserId == identityUserId &&
                item.BoardMember.IsActive);

        if (due == null)
        {
            return NotFound();
        }

        if (due.Status != MemberDueStatuses.Unpaid ||
            due.Payments.Any(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded))
        {
            TempData["ErrorMessage"] =
                "Bu aidat daha önce ödenmiş veya ödeme işlemine uygun değil.";
            return RedirectToAction(nameof(Index));
        }

        if (outcome == "failure")
        {
            TempData["ErrorMessage"] =
                "PayTR demo ödemesi başarısız olarak simüle edildi. Gerçek para çekilmedi.";
            return RedirectToAction(nameof(DemoCheckout), new { dueId = memberDueId });
        }

        var transactionDateUtc = DateTime.UtcNow;
        var referenceNumber = $"DEMO-{Guid.NewGuid():N}".ToUpperInvariant();

        due.Payments.Add(new DuesPayment
        {
            Amount = due.Amount,
            Currency = due.Currency,
            PaymentMethod = DuesPaymentMethods.OnlineCard,
            Status = DuesPaymentStatuses.Completed,
            Provider = "PayTR Demo",
            ProviderPaymentId = referenceNumber,
            ReferenceNumber = referenceNumber,
            PaidAtUtc = transactionDateUtc,
            CreatedAtUtc = transactionDateUtc,
            CreatedBy = identityUserId,
            AdminNote =
                "Yönetim kurulu üyesi tarafından geliştirme ortamında oluşturulan PayTR demo ödemesidir. Gerçek tahsilat değildir."
        });

        due.Status = MemberDueStatuses.Paid;
        due.PaidAtUtc = transactionDateUtc;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "PayTR demo ödemesi başarıyla tamamlandı. Gerçek para çekilmedi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<BoardMember?> GetCurrentBoardMemberAsync(
        string? identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return null;
        }

        return await _context.BoardMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(member =>
                member.IdentityUserId == identityUserId &&
                member.IsActive);
    }
}
