using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.AdminRole)]
public class DuesManagementController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DuesManagementController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.View)]
    public async Task<IActionResult> Index()
    {
        var periods = await _context.DuesPeriods
            .AsNoTracking()
            .Include(period => period.MemberDues)
                .ThenInclude(due => due.Payments)
            .OrderByDescending(period => period.StartDate)
            .ThenByDescending(period => period.Id)
            .ToListAsync();

        var validDues = periods
            .SelectMany(period => period.MemberDues)
            .Where(due =>
                due.Status != MemberDueStatuses.Cancelled &&
                due.Status != MemberDueStatuses.Exempt)
            .ToList();

        var collectedAmount = validDues
            .SelectMany(due => due.Payments)
            .Where(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded)
            .Sum(payment => payment.Amount - payment.RefundedAmount);

        var totalReceivable = validDues.Sum(due => due.Amount);

        return View(new DuesManagementIndexViewModel
        {
            PeriodCount = periods.Count,
            ActivePeriodCount = periods.Count(period => period.IsActive),
            TotalReceivable = totalReceivable,
            CollectedAmount = collectedAmount,
            OutstandingAmount = Math.Max(0, totalReceivable - collectedAmount),
            Periods = periods.Select(period => new DuesPeriodListItemViewModel
            {
                Id = period.Id,
                Name = period.Name,
                PeriodType = period.PeriodType,
                Amount = period.Amount,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                DueDate = period.DueDate,
                IsActive = period.IsActive,
                AssignedMemberCount = period.MemberDues.Count,
                PaidMemberCount = period.MemberDues.Count(due =>
                    due.Status == MemberDueStatuses.Paid),
                CollectedAmount = period.MemberDues
                    .SelectMany(due => due.Payments)
                    .Where(payment =>
                        payment.Status == DuesPaymentStatuses.Completed ||
                        payment.Status == DuesPaymentStatuses.PartiallyRefunded)
                    .Sum(payment => payment.Amount - payment.RefundedAmount)
            }).ToList()
        });
    }
    [HttpGet]
    [RequirePermission(
    ManagementArea.DuesAndPayments,
    PermissionActionType.View)]
    public async Task<IActionResult> Payments(
    string? search,
    string? status,
    string? paymentMethod)
    {
        search = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        status = string.IsNullOrWhiteSpace(status)
            ? null
            : status.Trim();

        paymentMethod = string.IsNullOrWhiteSpace(paymentMethod)
            ? null
            : paymentMethod.Trim();

        IQueryable<DuesPayment> query = _context.DuesPayments
            .AsNoTracking()
            .Include(payment => payment.MemberDue)
                .ThenInclude(due => due.MemberProfile)
            .Include(payment => payment.MemberDue)
                .ThenInclude(due => due.BoardMember)
            .Include(payment => payment.MemberDue)
                .ThenInclude(due => due.DuesPeriod);

        if (search != null)
        {
            query = query.Where(payment =>
                (payment.MemberDue.MemberProfile != null &&
                 payment.MemberDue.MemberProfile.DisplayName.Contains(search)) ||
                (payment.MemberDue.BoardMember != null &&
                 payment.MemberDue.BoardMember.FullName.Contains(search)) ||
                payment.MemberDue.DuesPeriod.Name.Contains(search) ||
                (payment.MemberDue.MemberProfile != null &&
                 payment.MemberDue.MemberProfile.RegistryNumber != null &&
                 payment.MemberDue.MemberProfile.RegistryNumber.Contains(search)) ||
                (payment.ReferenceNumber != null &&
                 payment.ReferenceNumber.Contains(search)));
        }

        if (status != null)
        {
            query = query.Where(payment =>
                payment.Status == status);
        }

        if (paymentMethod != null)
        {
            query = query.Where(payment =>
                payment.PaymentMethod == paymentMethod);
        }

        var paymentRecords = await query
            .OrderByDescending(payment => payment.PaidAtUtc)
            .ThenByDescending(payment => payment.Id)
            .ToListAsync();

        var payments = paymentRecords
            .Select(payment => new DuesPaymentHistoryItemViewModel
            {
                Id = payment.Id,
                MemberName =
                    payment.MemberDue.BoardMember != null
                        ? payment.MemberDue.BoardMember.FullName
                        : payment.MemberDue.MemberProfile!.DisplayName,
                RegistryNumber =
                    payment.MemberDue.MemberProfile != null
                        ? payment.MemberDue.MemberProfile.RegistryNumber
                        : null,
                PeriodName =
                    payment.MemberDue.DuesPeriod.Name,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Provider = payment.Provider,
                ReferenceNumber = payment.ReferenceNumber,
                PaidAtUtc = payment.PaidAtUtc,
                CreatedAtUtc = payment.CreatedAtUtc,
                IsDemo =
                    payment.Provider == "PayTR Demo" &&
                    payment.ReferenceNumber?.StartsWith(
                        "DEMO-",
                        StringComparison.OrdinalIgnoreCase) == true
            })
            .ToList();

        var totalCollected = paymentRecords
            .Where(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded)
            .Sum(payment =>
                payment.Amount - payment.RefundedAmount);

        return View(new DuesPaymentHistoryViewModel
        {
            Search = search,
            Status = status,
            PaymentMethod = paymentMethod,
            PaymentCount = payments.Count,
            TotalCollected = totalCollected,
            Payments = payments
        });
    }
    [HttpGet]
    [RequirePermission(
    ManagementArea.DuesAndPayments,
    PermissionActionType.View)]
    public async Task<IActionResult> PaymentReceipt(int id)
    {
        var payment = await _context.DuesPayments
            .AsNoTracking()
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.MemberProfile)
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.BoardMember)
            .Include(item => item.MemberDue)
                .ThenInclude(due => due.DuesPeriod)
            .SingleOrDefaultAsync(item => item.Id == id);

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
                "Yalnızca tamamlanmış ödeme işlemleri için ödeme belgesi oluşturulabilir.";

            return RedirectToAction(nameof(Payments));
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
            MemberName = payment.MemberDue.BoardMember != null
                ? payment.MemberDue.BoardMember.FullName
                : payment.MemberDue.MemberProfile!.DisplayName,
            RegistryNumber =
                payment.MemberDue.MemberProfile?.RegistryNumber,
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

    [HttpGet]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Add)]
    public IActionResult Create() => View(new DuesPeriodFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Add)]
    public async Task<IActionResult> Create(DuesPeriodFormViewModel model)
    {
        ValidatePeriod(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var period = new DuesPeriod();
        ApplyPeriodValues(period, model);
        period.CreatedAtUtc = DateTime.UtcNow;
        period.CreatedBy = User.Identity?.Name;

        _context.DuesPeriods.Add(period);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Aidat dönemi oluşturuldu. Şimdi faal üyelere tahakkuk ettirebilirsiniz.";
        return RedirectToAction(nameof(Details), new { id = period.Id });
    }

    [HttpGet]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var period = await _context.DuesPeriods
            .AsNoTracking()
            .Include(item => item.MemberDues)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (period == null)
        {
            return NotFound();
        }

        if (period.MemberDues.Count != 0)
        {
            TempData["ErrorMessage"] =
                "Üyelere tahakkuk ettirilmiş bir dönemin tutar ve tarihleri değiştirilemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(new DuesPeriodFormViewModel
        {
            Id = period.Id,
            Name = period.Name,
            PeriodType = period.PeriodType,
            Amount = period.Amount,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            DueDate = period.DueDate,
            Description = period.Description,
            IsActive = period.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, DuesPeriodFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var period = await _context.DuesPeriods
            .Include(item => item.MemberDues)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (period == null)
        {
            return NotFound();
        }

        if (period.MemberDues.Count != 0)
        {
            TempData["ErrorMessage"] =
                "Üyelere tahakkuk ettirilmiş bir dönem değiştirilemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        ValidatePeriod(model);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplyPeriodValues(period, model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Aidat dönemi güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.View)]
    public async Task<IActionResult> Details(int id)
    {
        var period = await _context.DuesPeriods
            .AsNoTracking()
            .Include(item => item.MemberDues)
                .ThenInclude(due => due.MemberProfile)
            .Include(item => item.MemberDues)
                .ThenInclude(due => due.BoardMember)
            .Include(item => item.MemberDues)
                .ThenInclude(due => due.Payments)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (period == null)
        {
            return NotFound();
        }

        var activeMemberCount =
            await _context.MemberProfiles.CountAsync(member => member.IsActive) +
            await _context.BoardMembers.CountAsync(member => member.IsActive);
        var today = DateTime.Today;
        var dues = period.MemberDues
            .OrderBy(due => due.BoardMember != null
                ? due.BoardMember.FullName
                : due.MemberProfile!.DisplayName)
            .Select(due =>
            {
                var payment = due.Payments
                    .Where(item =>
                        item.Status == DuesPaymentStatuses.Completed ||
                        item.Status == DuesPaymentStatuses.PartiallyRefunded)
                    .OrderByDescending(item => item.PaidAtUtc)
                    .FirstOrDefault();

                return new MemberDueAdminItemViewModel
                {
                    Id = due.Id,
                    MemberName = due.BoardMember != null
                        ? due.BoardMember.FullName
                        : due.MemberProfile!.DisplayName,
                    RegistryNumber = due.MemberProfile?.RegistryNumber,
                    MemberType = due.BoardMember != null
                        ? "Yönetim Kurulu"
                        : due.MemberProfile!.MemberType,
                    Amount = due.Amount,
                    Status = due.Status,
                    AssignedAtUtc = due.AssignedAtUtc,
                    PaidAtUtc = due.PaidAtUtc,
                    PaymentMethod = payment?.PaymentMethod,
                    ReferenceNumber = payment?.ReferenceNumber,
                    HasDemoPayment =
                        payment?.Provider == "PayTR Demo" &&
                        payment.ReferenceNumber?.StartsWith(
                            "DEMO-",
                            StringComparison.OrdinalIgnoreCase) == true,
                    IsOverdue = due.Status == MemberDueStatuses.Unpaid &&
                        period.DueDate.Date < today
                };
            })
            .ToList();

        var collectedAmount = period.MemberDues
            .SelectMany(due => due.Payments)
            .Where(payment =>
                payment.Status == DuesPaymentStatuses.Completed ||
                payment.Status == DuesPaymentStatuses.PartiallyRefunded)
            .Sum(payment => payment.Amount - payment.RefundedAmount);

        var receivable = period.MemberDues
            .Where(due =>
                due.Status != MemberDueStatuses.Cancelled &&
                due.Status != MemberDueStatuses.Exempt)
            .Sum(due => due.Amount);

        return View(new DuesPeriodDetailsViewModel
        {
            Id = period.Id,
            Name = period.Name,
            PeriodType = period.PeriodType,
            Amount = period.Amount,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            DueDate = period.DueDate,
            Description = period.Description,
            IsActive = period.IsActive,
            ActiveMemberCount = activeMemberCount,
            AssignedMemberCount = period.MemberDues.Count,
            PaidMemberCount = period.MemberDues.Count(due =>
                due.Status == MemberDueStatuses.Paid),
            CollectedAmount = collectedAmount,
            OutstandingAmount = Math.Max(0, receivable - collectedAmount),
            DemoResetEnabled = _environment.IsDevelopment(),
            MemberDues = dues
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Delete)]
    public async Task<IActionResult> ResetDemoPayment(int id)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var due = await _context.MemberDues
            .Include(item => item.Payments)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (due == null)
        {
            return NotFound();
        }

        var demoPayments = due.Payments
            .Where(payment =>
                payment.Provider == "PayTR Demo" &&
                payment.ReferenceNumber?.StartsWith(
                    "DEMO-",
                    StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (demoPayments.Count == 0)
        {
            TempData["ErrorMessage"] =
                "Bu aidatta sıfırlanabilecek bir PayTR demo ödemesi bulunamadı.";
            return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
        }

        if (due.Payments.Any(payment => !demoPayments.Contains(payment)))
        {
            TempData["ErrorMessage"] =
                "Bu aidatta demo dışında ödeme kaydı bulunduğu için sıfırlama yapılamadı.";
            return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
        }

        _context.DuesPayments.RemoveRange(demoPayments);
        due.Status = MemberDueStatuses.Unpaid;
        due.PaidAtUtc = null;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "PayTR demo ödemesi sıfırlandı. Üye demo ödeme işlemini yeniden deneyebilir.";
        return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Approve)]
    public async Task<IActionResult> AssignToActiveMembers(int id)
    {
        var period = await _context.DuesPeriods
            .SingleOrDefaultAsync(item => item.Id == id);

        if (period == null)
        {
            return NotFound();
        }

        if (!period.IsActive)
        {
            TempData["ErrorMessage"] =
                "Pasif bir aidat dönemi üyelere tahakkuk ettirilemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var existingMemberIds = await _context.MemberDues
            .Where(due =>
                due.DuesPeriodId == id &&
                due.MemberProfileId != null)
            .Select(due => due.MemberProfileId!.Value)
            .ToListAsync();

        var members = await _context.MemberProfiles
            .Where(member =>
                member.IsActive &&
                !existingMemberIds.Contains(member.Id))
            .Select(member => member.Id)
            .ToListAsync();

        foreach (var memberId in members)
        {
            _context.MemberDues.Add(new MemberDue
            {
                DuesPeriodId = period.Id,
                MemberProfileId = memberId,
                Amount = period.Amount,
                Currency = period.Currency,
                Status = MemberDueStatuses.Unpaid,
                AssignedAtUtc = DateTime.UtcNow
            });
        }

        var existingBoardMemberIds = await _context.MemberDues
            .Where(due =>
                due.DuesPeriodId == id &&
                due.BoardMemberId != null)
            .Select(due => due.BoardMemberId!.Value)
            .ToListAsync();

        var boardMembers = await _context.BoardMembers
            .Where(member =>
                member.IsActive &&
                !existingBoardMemberIds.Contains(member.Id))
            .Select(member => member.Id)
            .ToListAsync();

        foreach (var boardMemberId in boardMembers)
        {
            _context.MemberDues.Add(new MemberDue
            {
                DuesPeriodId = period.Id,
                BoardMemberId = boardMemberId,
                Amount = period.Amount,
                Currency = period.Currency,
                Status = MemberDueStatuses.Unpaid,
                AssignedAtUtc = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        var assignedCount = members.Count + boardMembers.Count;
        TempData["SuccessMessage"] = assignedCount == 0
            ? "Bu dönemin aidatı bütün faal üyelere ve yönetim kurulu üyelerine daha önce tahakkuk ettirilmiş."
            : $"Aidat {members.Count} faal üyeye ve {boardMembers.Count} yönetim kurulu üyesine tahakkuk ettirildi.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Edit)]
    public async Task<IActionResult> RecordPayment(int id)
    {
        var due = await GetDueForPaymentAsync(id);
        if (due == null)
        {
            return NotFound();
        }

        if (due.Status == MemberDueStatuses.Paid)
        {
            TempData["ErrorMessage"] = "Bu aidat zaten ödenmiş görünüyor.";
            return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
        }

        return View(CreatePaymentModel(due));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Edit)]
    public async Task<IActionResult> RecordPayment(ManualDuesPaymentViewModel model)
    {
        var due = await GetDueForPaymentAsync(model.MemberDueId);
        if (due == null)
        {
            return NotFound();
        }

        if (due.Status == MemberDueStatuses.Paid)
        {
            TempData["ErrorMessage"] = "Bu aidat için ikinci kez ödeme kaydı oluşturulamaz.";
            return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
        }

        if (!DuesPaymentMethods.ManualMethods.Contains(model.PaymentMethod))
        {
            ModelState.AddModelError(
                nameof(model.PaymentMethod),
                "Geçerli bir manuel ödeme yöntemi seçmelisiniz.");
        }

        if (model.PaidAtLocal > DateTime.Now.AddMinutes(5))
        {
            ModelState.AddModelError(
                nameof(model.PaidAtLocal),
                "Ödeme tarihi gelecekte olamaz.");
        }

        if (!ModelState.IsValid)
        {
            ApplyPaymentReadOnlyValues(model, due);
            return View(model);
        }

        var paidAtLocal = DateTime.SpecifyKind(
            model.PaidAtLocal,
            DateTimeKind.Local);

        _context.DuesPayments.Add(new DuesPayment
        {
            MemberDueId = due.Id,
            Amount = due.Amount,
            Currency = due.Currency,
            PaymentMethod = model.PaymentMethod,
            Status = DuesPaymentStatuses.Completed,
            Provider = "Manuel",
            ReferenceNumber = Clean(model.ReferenceNumber),
            PaidAtUtc = paidAtLocal.ToUniversalTime(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name,
            AdminNote = Clean(model.AdminNote)
        });

        due.Status = MemberDueStatuses.Paid;
        due.PaidAtUtc = paidAtLocal.ToUniversalTime();
        due.AdminNote = Clean(model.AdminNote);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Ödeme, aidat tutarı değiştirilmeden başarıyla kaydedildi.";
        return RedirectToAction(nameof(Details), new { id = due.DuesPeriodId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.DuesAndPayments, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var period = await _context.DuesPeriods
            .Include(item => item.MemberDues)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (period == null)
        {
            return NotFound();
        }

        if (period.MemberDues.Count != 0)
        {
            TempData["ErrorMessage"] =
                "Tahakkuk kaydı bulunan aidat dönemi silinemez.";
            return RedirectToAction(nameof(Details), new { id });
        }

        _context.DuesPeriods.Remove(period);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Aidat dönemi silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<MemberDue?> GetDueForPaymentAsync(int id) =>
        await _context.MemberDues
            .Include(due => due.DuesPeriod)
            .Include(due => due.MemberProfile)
            .Include(due => due.BoardMember)
            .SingleOrDefaultAsync(due => due.Id == id);

    private static ManualDuesPaymentViewModel CreatePaymentModel(MemberDue due)
    {
        var model = new ManualDuesPaymentViewModel();
        ApplyPaymentReadOnlyValues(model, due);
        return model;
    }

    private static void ApplyPaymentReadOnlyValues(
        ManualDuesPaymentViewModel model,
        MemberDue due)
    {
        model.MemberDueId = due.Id;
        model.DuesPeriodId = due.DuesPeriodId;
        model.PeriodName = due.DuesPeriod.Name;
        model.MemberName = due.BoardMember != null
            ? due.BoardMember.FullName
            : due.MemberProfile!.DisplayName;
        model.RegistryNumber = due.MemberProfile?.RegistryNumber;
        model.Amount = due.Amount;
    }

    private void ValidatePeriod(DuesPeriodFormViewModel model)
    {
        if (!DuesPeriodTypes.All.Contains(model.PeriodType))
        {
            ModelState.AddModelError(
                nameof(model.PeriodType),
                "Geçerli bir dönem türü seçmelisiniz.");
        }

        if (model.EndDate.Date < model.StartDate.Date)
        {
            ModelState.AddModelError(
                nameof(model.EndDate),
                "Dönem bitişi başlangıç tarihinden önce olamaz.");
        }

        if (model.DueDate.Date < model.StartDate.Date)
        {
            ModelState.AddModelError(
                nameof(model.DueDate),
                "Son ödeme tarihi iki dönem arasında olamaz.");
        }
    
    }

    private static void ApplyPeriodValues(
        DuesPeriod period,
        DuesPeriodFormViewModel model)
    {
        period.Name = model.Name.Trim();
        period.PeriodType = model.PeriodType;
        period.Amount = model.Amount;
        period.Currency = "TRY";
        period.StartDate = model.StartDate.Date;
        period.EndDate = model.EndDate.Date;
        period.DueDate = model.DueDate.Date;
        period.Description = Clean(model.Description);
        period.IsActive = model.IsActive;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
