using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Controllers;

[Authorize(Roles = IdentitySeedData.ManagementRoles)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index() => RedirectToAction(
        "Index",
        "Dashboard",
        new { area = "Admin" });

    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.View)]
    public async Task<IActionResult> Volunteers(string? search, string? status)
    {
        var query = _context.Volunteers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(item =>
                item.FullName.Contains(term) ||
                item.Email.Contains(term) ||
                item.Phone.Contains(term) ||
                item.ExpertiseArea.Contains(term));
        }

        var volunteers = await query
            .OrderByDescending(volunteer => volunteer.CreatedDate)
            .ToListAsync();

        var volunteerPageId = await GetVolunteerPageIdAsync();
        var metadata = volunteerPageId == null
            ? new Dictionary<int, VolunteerApplicationMetadata>()
            : (await _context.CorporateContentSections.AsNoTracking()
                .Where(item =>
                    item.CorporatePageId == volunteerPageId &&
                    item.SectionKey.StartsWith(VolunteerSeedData.ApplicationPrefix))
                .ToListAsync())
                .Select(item => new
                {
                    Id = ParseVolunteerId(item.SectionKey),
                    Metadata = DeserializeVolunteerMetadata(item.ContentHtml)
                })
                .Where(item => item.Id.HasValue)
                .ToDictionary(item => item.Id!.Value, item => item.Metadata);

        var items = volunteers.Select(item => new VolunteerApplicationAdminItemViewModel
        {
            Application = item,
            Metadata = metadata.GetValueOrDefault(item.Id) ?? new VolunteerApplicationMetadata()
        });

        if (!string.IsNullOrWhiteSpace(status))
        {
            items = items.Where(item => item.Metadata.Status == status);
        }

        var materializedItems = items.ToList();
        var allVolunteerIds = await _context.Volunteers.AsNoTracking()
            .Select(item => item.Id)
            .ToListAsync();
        return View(new VolunteerApplicationsAdminViewModel
        {
            Items = materializedItems,
            Search = search,
            Status = status,
            TotalCount = allVolunteerIds.Count,
            NewCount = allVolunteerIds.Count(id =>
                (metadata.GetValueOrDefault(id)?.Status ?? "Yeni") == "Yeni")
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Approve)]
    public async Task<IActionResult> UpdateVolunteer(int id, string status, string? adminNote)
    {
        var allowedStatuses = new[] { "Yeni", "İnceleniyor", "İletişime Geçildi", "Onaylandı", "Uygun Değil", "Arşivlendi" };
        if (!allowedStatuses.Contains(status)) return BadRequest();
        var volunteer = await _context.Volunteers.SingleOrDefaultAsync(item => item.Id == id);
        var pageId = await GetVolunteerPageIdAsync();
        if (volunteer == null || pageId == null) return NotFound();

        var key = $"{VolunteerSeedData.ApplicationPrefix}{id}";
        var section = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.CorporatePageId == pageId && item.SectionKey == key);
        var metadata = new VolunteerApplicationMetadata
        {
            Status = status,
            AdminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim()
        };

        if (section == null)
        {
            _context.CorporateContentSections.Add(new CorporateContentSection
            {
                CorporatePageId = pageId.Value,
                SectionKey = key,
                Title = $"{volunteer.FullName} gönüllü başvurusu",
                ContentHtml = JsonSerializer.Serialize(metadata),
                DisplayOrder = volunteer.Id,
                IsActive = false,
                UpdatedBy = User.Identity?.Name
            });
        }
        else
        {
            section.ContentHtml = JsonSerializer.Serialize(metadata);
            section.UpdatedAt = DateTime.UtcNow;
            section.UpdatedBy = User.Identity?.Name;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gönüllü başvurusu güncellendi.";
        return RedirectToAction(nameof(Volunteers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteVolunteer(int id)
    {
        var volunteer = await _context.Volunteers.SingleOrDefaultAsync(item => item.Id == id);
        if (volunteer == null) return NotFound();
        var pageId = await GetVolunteerPageIdAsync();
        if (pageId != null)
        {
            var key = $"{VolunteerSeedData.ApplicationPrefix}{id}";
            var section = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
                item.CorporatePageId == pageId && item.SectionKey == key);
            if (section != null) _context.CorporateContentSections.Remove(section);
        }

        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gönüllü başvurusu silindi.";
        return RedirectToAction(nameof(Volunteers));
    }

    [RequirePermission(
        ManagementArea.DonationManagement,
        PermissionActionType.View)]
    public async Task<IActionResult> Donations()
    {
        var donations = await _context.Donations
            .AsNoTracking()
            .OrderByDescending(donation => donation.DonationDate)
            .ToListAsync();

        return View(donations);
    }
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.View)]
    public async Task<IActionResult> MembershipApplications()
    {
        var applications = await _context.MembershipApplications
            .AsNoTracking()
            .Include(application => application.MemberProfile)
            .OrderByDescending(application => application.CreatedAt)
            .ToListAsync();

        return View(applications);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteMembershipApplication(int id)
    {
        var application = await _context.MembershipApplications
            .Include(item => item.MemberProfile)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (application == null)
        {
            return NotFound();
        }

        if (application.MemberProfile != null ||
            application.ApplicationStatus ==
            MembershipApplicationStatuses.Approved)
        {
            TempData["ErrorMessage"] =
                "Onaylanmış bir üyelik başvurusu silinemez. Üyelik kaydı ve işlem geçmişi korunmalıdır.";

            return RedirectToAction(nameof(MembershipApplications));
        }

        _context.MembershipApplications.Remove(application);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Üyelik başvurusu kalıcı olarak silindi.";

        return RedirectToAction(nameof(MembershipApplications));
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Approve)]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Add)]
    public async Task<IActionResult> ApproveMembershipApplication(int id)
    {
        var application = await _context.MembershipApplications
            .AsNoTracking()
            .Include(item => item.MemberProfile)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (application == null)
        {
            return NotFound();
        }

        if (application.MemberProfile != null ||
            application.ApplicationStatus ==
            MembershipApplicationStatuses.Approved)
        {
            TempData["ErrorMessage"] =
                "Bu başvuru daha önce onaylanmış.";

            return RedirectToAction(nameof(MembershipApplications));
        }

        var existingUser = await _userManager.FindByEmailAsync(
            application.Email.Trim());

        if (existingUser != null &&
            await _context.MemberProfiles.AsNoTracking().AnyAsync(
                member => member.IdentityUserId == existingUser.Id))
        {
            TempData["ErrorMessage"] =
                "Bu e-posta adresine bağlı bir üye kaydı zaten var.";

            return RedirectToAction(nameof(MembershipApplications));
        }

        return View(CreateApprovalModel(
            application,
            existingUser != null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Approve)]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Add)]
    public async Task<IActionResult> ApproveMembershipApplication(
        MembershipApplicationApprovalViewModel model)
    {
        var application = await _context.MembershipApplications
            .Include(item => item.MemberProfile)
            .SingleOrDefaultAsync(item =>
                item.Id == model.ApplicationId);

        if (application == null)
        {
            return NotFound();
        }

        if (application.MemberProfile != null ||
            application.ApplicationStatus ==
            MembershipApplicationStatuses.Approved)
        {
            TempData["ErrorMessage"] =
                "Bu başvuru daha önce onaylanmış.";

            return RedirectToAction(nameof(MembershipApplications));
        }

        var identityUser = await _userManager.FindByEmailAsync(
            application.Email.Trim());

        model.ApplicantName = application.ApplicantName;
        model.MembershipType = application.MembershipType;
        model.Email = application.Email;
        model.Phone = application.Phone;
        model.City = application.City;
        model.HasExistingAccount = identityUser != null;

        if (identityUser != null &&
            await _context.MemberProfiles.AsNoTracking().AnyAsync(
                member => member.IdentityUserId == identityUser.Id))
        {
            ModelState.AddModelError(
                string.Empty,
                "Bu e-posta adresine bağlı bir üye kaydı zaten var.");
        }

        if (identityUser == null &&
            string.IsNullOrWhiteSpace(model.TemporaryPassword))
        {
            ModelState.AddModelError(
                nameof(model.TemporaryPassword),
                "Yeni giriş hesabı için geçici şifre zorunludur.");
        }

        var registryNumber = Clean(model.RegistryNumber);

        if (registryNumber != null &&
            await _context.MemberProfiles.AsNoTracking().AnyAsync(
                member => member.RegistryNumber == registryNumber))
        {
            ModelState.AddModelError(
                nameof(model.RegistryNumber),
                "Bu üye numarası başka bir kayıtta kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var accountWasCreated = false;

        if (identityUser == null)
        {
            identityUser = new IdentityUser
            {
                UserName = application.Email.Trim(),
                Email = application.Email.Trim(),
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(
                identityUser,
                model.TemporaryPassword!);

            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);
                return View(model);
            }

            accountWasCreated = true;
        }
        else if (!identityUser.EmailConfirmed)
        {
            identityUser.EmailConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(identityUser);

            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);
                return View(model);
            }
        }

        if (!await _userManager.IsInRoleAsync(
                identityUser,
                IdentitySeedData.MemberRole))
        {
            var roleResult = await _userManager.AddToRoleAsync(
                identityUser,
                IdentitySeedData.MemberRole);

            if (!roleResult.Succeeded)
            {
                AddIdentityErrors(roleResult);
                return View(model);
            }
        }

        var now = DateTime.UtcNow;
        var member = new MemberProfile
        {
            MemberType = application.MembershipType,
            DisplayName = application.ApplicantName.Trim(),
            RegistryNumber = registryNumber,
            Specialty = model.Specialty.Trim(),
            City = application.City.Trim(),
            Region = Clean(model.Region),
            ShortDescription = Clean(model.ShortDescription),
            Phone = application.Phone.Trim(),
            IdentityUserId = identityUser.Id,
            MembershipApplicationId = application.Id,
            IsActive = true,
            HasPublicationConsent = false,
            IsPublished = false,
            CreatedAt = now,
            MembershipApprovedAtUtc = now
        };

        _context.MemberProfiles.Add(member);

        application.ApplicationStatus =
            MembershipApplicationStatuses.Approved;
        application.ReviewedAtUtc = now;
        application.ReviewedBy = User.Identity?.Name;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["SuccessMessage"] = accountWasCreated
            ? "Başvuru onaylandı; üye profili ve giriş hesabı oluşturuldu. Geçici şifreyi üyeye güvenli bir kanaldan iletin."
            : "Başvuru onaylandı; mevcut giriş hesabı üye profiline bağlandı ve Member rolü verildi.";

        return RedirectToAction(nameof(MembershipApplications));
    }

    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.View)]
    public async Task<IActionResult> ContactMessages(string? search, string? status)
    {
        var query = _context.ContactMessages
            .AsNoTracking()
            .Include(message => message.RecipientMemberProfile)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(message =>
                message.FullName.Contains(term) ||
                message.Email.Contains(term) ||
                message.Subject.Contains(term) ||
                message.Message.Contains(term) ||
                (message.RecipientDisplayName != null &&
                 message.RecipientDisplayName.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(message => message.Status == status);
        }

        var messages = await query
            .AsNoTracking()
            .OrderByDescending(message => message.CreatedAt)
            .ToListAsync();

        var contactPageId = await GetContactPageIdAsync();
        var notes = contactPageId == null
            ? new Dictionary<int, string?>()
            : (await _context.CorporateContentSections.AsNoTracking()
                .Where(item =>
                    item.CorporatePageId == contactPageId &&
                    item.SectionKey.StartsWith("message-note-"))
                .ToListAsync())
                .Select(item => new
                {
                    Id = ParseMessageId(item.SectionKey),
                    Note = item.ContentHtml
                })
                .Where(item => item.Id.HasValue)
                .ToDictionary(item => item.Id!.Value, item => item.Note);

        return View(new ContactMessagesAdminViewModel
        {
            Items = messages.Select(message => new ContactMessageAdminItemViewModel
            {
                Message = message,
                AdminNote = notes.GetValueOrDefault(message.Id)
            }).ToList(),
            Search = search,
            Status = status,
            TotalCount = await _context.ContactMessages.CountAsync(),
            NewCount = await _context.ContactMessages.CountAsync(message => message.Status == "Yeni"),
            MemberMessageCount = await _context.ContactMessages.CountAsync(
                message => message.RecipientDisplayName != null)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Edit)]
    public async Task<IActionResult> UpdateContactMessage(int id, string status, string? adminNote)
    {
        var allowedStatuses = new[] { "Yeni", "Okundu", "Yanıtlandı", "Arşivlendi" };
        if (!allowedStatuses.Contains(status)) return BadRequest();

        var message = await _context.ContactMessages.SingleOrDefaultAsync(item => item.Id == id);
        var contactPageId = await GetContactPageIdAsync();
        if (message == null || contactPageId == null) return NotFound();

        message.Status = status;
        var noteKey = $"message-note-{id}";
        var note = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.CorporatePageId == contactPageId && item.SectionKey == noteKey);

        if (string.IsNullOrWhiteSpace(adminNote))
        {
            if (note != null) _context.CorporateContentSections.Remove(note);
        }
        else if (note == null)
        {
            _context.CorporateContentSections.Add(new CorporateContentSection
            {
                CorporatePageId = contactPageId.Value,
                SectionKey = noteKey,
                Title = $"{message.FullName} için yönetici notu",
                ContentHtml = adminNote.Trim(),
                DisplayOrder = message.Id,
                IsActive = false,
                UpdatedBy = User.Identity?.Name
            });
        }
        else
        {
            note.ContentHtml = adminNote.Trim();
            note.UpdatedAt = DateTime.UtcNow;
            note.UpdatedBy = User.Identity?.Name;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Mesaj durumu ve yönetici notu güncellendi.";
        return RedirectToAction(nameof(ContactMessages));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ApplicationsAndCommunication,
        PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteContactMessage(int id)
    {
        var message = await _context.ContactMessages.SingleOrDefaultAsync(item => item.Id == id);
        if (message == null) return NotFound();

        var contactPageId = await GetContactPageIdAsync();
        if (contactPageId != null)
        {
            var noteKey = $"message-note-{id}";
            var note = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
                item.CorporatePageId == contactPageId && item.SectionKey == noteKey);
            if (note != null) _context.CorporateContentSections.Remove(note);
        }

        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "İletişim mesajı silindi.";
        return RedirectToAction(nameof(ContactMessages));
    }

    [RequirePermission(
        ManagementArea.ConfidentialReports,
        PermissionActionType.View)]
    public async Task<IActionResult> ConfidentialReports(string? search, string? status)
    {
        var query = _context.ConfidentialReports
            .AsNoTracking()
            .Include(report => report.Attachments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(report =>
                report.Category.Contains(term) ||
                report.InsuranceBranch.Contains(term) ||
                report.Details.Contains(term) ||
                (report.City != null && report.City.Contains(term)) ||
                (report.ReferenceInformation != null && report.ReferenceInformation.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(report => report.Status == status);
        }

        return View(new ConfidentialReportsAdminViewModel
        {
            Items = await query
                .OrderByDescending(report => report.CreatedAtUtc)
                .ToListAsync(),
            Search = search,
            Status = status,
            TotalCount = await _context.ConfidentialReports.CountAsync(),
            NewCount = await _context.ConfidentialReports.CountAsync(report => report.Status == "Yeni")
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ConfidentialReports,
        PermissionActionType.Edit)]
    public async Task<IActionResult> UpdateConfidentialReport(
        int id,
        string status,
        string? adminNote)
    {
        var allowedStatuses = new[]
        {
            "Yeni",
            "İnceleniyor",
            "Değerlendirildi",
            "İşlem Tamamlandı",
            "Arşivlendi"
        };

        if (!allowedStatuses.Contains(status)) return BadRequest();

        var report = await _context.ConfidentialReports
            .SingleOrDefaultAsync(item => item.Id == id);
        if (report == null) return NotFound();

        report.Status = status;
        report.AdminNote = string.IsNullOrWhiteSpace(adminNote)
            ? null
            : adminNote.Trim();

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gizli ihbar kaydı güncellendi.";
        return RedirectToAction(nameof(ConfidentialReports));
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.ConfidentialReports,
        PermissionActionType.View)]
    public async Task<IActionResult> DownloadConfidentialReportAttachment(int id)
    {
        var attachment = await _context.ConfidentialReportAttachments
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);
        if (attachment == null) return NotFound();

        return File(
            attachment.Content,
            "application/octet-stream",
            Path.GetFileName(attachment.OriginalFileName));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.ConfidentialReports,
        PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteConfidentialReport(int id)
    {
        var report = await _context.ConfidentialReports
            .SingleOrDefaultAsync(item => item.Id == id);
        if (report == null) return NotFound();

        _context.ConfidentialReports.Remove(report);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gizli ihbar kaydı silindi.";
        return RedirectToAction(nameof(ConfidentialReports));
    }

    private static MembershipApplicationApprovalViewModel CreateApprovalModel(
        MembershipApplication application,
        bool hasExistingAccount)
    {
        return new MembershipApplicationApprovalViewModel
        {
            ApplicationId = application.Id,
            ApplicantName = application.ApplicantName,
            MembershipType = application.MembershipType,
            Email = application.Email,
            Phone = application.Phone,
            City = application.City,
            Specialty = application.Specialty ?? string.Empty,
            ShortDescription = application.Message,
            HasExistingAccount = hasExistingAccount
        };
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private async Task<int?> GetContactPageIdAsync() => await _context.CorporatePages
        .Where(item => item.PageKey == ContactAndFaqSeedData.ContactPageKey)
        .Select(item => (int?)item.Id)
        .SingleOrDefaultAsync();

    private static int? ParseMessageId(string key) =>
        int.TryParse(key["message-note-".Length..], out var id) ? id : null;

    private async Task<int?> GetVolunteerPageIdAsync() => await _context.CorporatePages
        .Where(item => item.PageKey == VolunteerSeedData.PageKey)
        .Select(item => (int?)item.Id)
        .SingleOrDefaultAsync();

    private static int? ParseVolunteerId(string key) =>
        int.TryParse(key[VolunteerSeedData.ApplicationPrefix.Length..], out var id) ? id : null;

    private static VolunteerApplicationMetadata DeserializeVolunteerMetadata(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new VolunteerApplicationMetadata();
        try { return JsonSerializer.Deserialize<VolunteerApplicationMetadata>(json) ?? new VolunteerApplicationMetadata(); }
        catch (JsonException) { return new VolunteerApplicationMetadata(); }
    }
}
