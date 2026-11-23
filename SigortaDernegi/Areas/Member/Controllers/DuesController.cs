using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SigortaDernegi.Areas.Member.Authorization;
using SigortaDernegi.Areas.Member.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Policy = MemberPortalPolicies.ActiveMember)]
public class DuesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DuesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var memberId = await _context.MemberProfiles
            .AsNoTracking()
            .Where(member =>
                member.IdentityUserId == identityUserId &&
                member.IsActive)
            .Select(member => (int?)member.Id)
            .SingleOrDefaultAsync();

        if (!memberId.HasValue)
        {
            return Forbid();
        }

        var dues = await _context.MemberDues
            .AsNoTracking()
            .Where(due =>
                due.MemberProfileId == memberId.Value &&
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

        var validDues = dues.Where(due =>
            due.Status != MemberDueStatuses.Exempt).ToList();

        return View(new MemberDuesViewModel
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
        });
    }

    [HttpGet]
    public async Task<IActionResult> Payments()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Challenge();
        }

        var payments = await _context.DuesPayments
            .AsNoTracking()
            .Where(payment =>
                payment.MemberDue.MemberProfile.IdentityUserId == identityUserId &&
                payment.MemberDue.MemberProfile.IsActive)
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

        var netPaidAmount = payments
            .Where(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded)
            .Sum(payment => Math.Max(
                0,
                payment.Amount - payment.RefundedAmount));

        return View(new MemberPaymentHistoryViewModel
        {
            PaymentCount = payments.Count,
            NetPaidAmount = netPaidAmount,
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
                .ThenInclude(due => due.MemberProfile)
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.DuesPeriod)
            .SingleOrDefaultAsync(item =>
                item.Id == id &&
                item.MemberDue.MemberProfile.IdentityUserId == identityUserId &&
                item.MemberDue.MemberProfile.IsActive);

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

        var documentNumber =
            string.IsNullOrWhiteSpace(payment.ReferenceNumber)
                ? $"ODEME-{payment.Id:D6}"
                : payment.ReferenceNumber;

        var model = new DuesPaymentReceiptViewModel
        {
            PaymentId = payment.Id,
            DuesPeriodId = payment.MemberDue.DuesPeriodId,
            DocumentNumber = documentNumber,
            MemberName = payment.MemberDue.MemberProfile.DisplayName,
            RegistryNumber = payment.MemberDue.MemberProfile.RegistryNumber,
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
        };

        return View(model);
    }
}
