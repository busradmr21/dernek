using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SigortaDernegi.Areas.Member.Authorization;
using SigortaDernegi.Areas.Member.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Policy = MemberPortalPolicies.ActiveMember)]
public class DemoPaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DemoPaymentController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int dueId)
    {
        // Demo ödeme işlemi yalnızca geliştirme ortamında çalışır.
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var identityUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Challenge();
        }

        var due = await _context.MemberDues
            .AsNoTracking()
            .Include(item => item.DuesPeriod)
            .Include(item => item.MemberProfile)
            .SingleOrDefaultAsync(item =>
                item.Id == dueId &&
                item.MemberProfile.IdentityUserId == identityUserId &&
                item.MemberProfile.IsActive);

        if (due == null)
        {
            return NotFound();
        }

        // Ödenmiş, muaf veya iptal edilmiş aidatlar tekrar ödenemez.
        if (due.Status != MemberDueStatuses.Unpaid)
        {
            return RedirectToAction("Index", "Dues");
        }

        var model = new DemoPaymentViewModel
        {
            MemberDueId = due.Id,
            MemberName = due.MemberProfile.DisplayName,
            PeriodName = due.DuesPeriod.Name,
            Amount = due.Amount,
            DueDate = due.DuesPeriod.DueDate,
            IsOverdue = due.DuesPeriod.DueDate.Date < DateTime.Today
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Simulate(
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

        var identityUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Challenge();
        }

        var due = await _context.MemberDues
            .Include(item => item.MemberProfile)
            .Include(item => item.Payments)
            .SingleOrDefaultAsync(item =>
                item.Id == memberDueId &&
                item.MemberProfile.IdentityUserId == identityUserId &&
                item.MemberProfile.IsActive);

        if (due == null)
        {
            return NotFound();
        }

        // Ödenmiş, muaf veya iptal edilmiş aidat tekrar işlenemez.
        if (due.Status != MemberDueStatuses.Unpaid ||
            due.Payments.Any(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded))
        {
            TempData["ErrorMessage"] =
                "Bu aidat daha önce ödenmiş veya ödeme işlemine uygun değil.";

            return RedirectToAction("Index", "Dues");
        }

        // Başarısız simülasyonda aidat ve ödeme kayıtları değiştirilmez.
        if (outcome == "failure")
        {
            TempData["ErrorMessage"] =
                "PayTR demo ödemesi başarısız olarak simüle edildi. Gerçek para çekilmedi.";

            return RedirectToAction(
                nameof(Checkout),
                new { dueId = memberDueId });
        }

        var transactionDateUtc = DateTime.UtcNow;
        var referenceNumber =
            $"DEMO-{Guid.NewGuid():N}".ToUpperInvariant();

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
                "Geliştirme ortamında oluşturulan PayTR demo ödemesidir. Gerçek tahsilat değildir."
        });

        due.Status = MemberDueStatuses.Paid;
        due.PaidAtUtc = transactionDateUtc;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "PayTR demo ödemesi başarıyla tamamlandı. Gerçek para çekilmedi.";

        return RedirectToAction("Index", "Dues");
    }
}