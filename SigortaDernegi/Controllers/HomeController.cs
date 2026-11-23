using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers;

public class HomeController : Controller
{
    private const int MaximumReportFileCount = 5;
    private const long MaximumReportFileSize = 10 * 1024 * 1024;
    private const long MaximumReportTotalSize = 20 * 1024 * 1024;

    private static readonly string[] ReportBranches =
    {
        "Kara Araçları",
        "Yangın ve Mühendislik",
        "Sağlık, Hayat ve Ferdi Kaza",
        "Denizcilik, Havacılık ve Raylı Sistemler",
        "Sorumluluk ve Diğer"
    };

    private static readonly string[] ReportCategories =
    {
        "Organize Hasar",
        "Kurgusal Kaza / Sahte Tutanak",
        "Sahte Belge / Rapor",
        "Mükerrer veya Şüpheli Hasar",
        "Eksper Raporu Uyuşmazlığı",
        "Diğer"
    };

    private static readonly HashSet<string> AllowedReportContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

    private static readonly HashSet<string> AllowedReportExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };

    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildHomePageViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("public-forms")]
    public async Task<IActionResult> SubmitHomeContact(
        [Bind(Prefix = "ContactForm")] HomeContactFormViewModel form,
        string? contactWebsite)
    {
        var contactSettings = await GetContactSettingsAsync();

        if (!string.IsNullOrWhiteSpace(contactWebsite))
        {
            TempData["HomeContactSuccess"] = contactSettings.SuccessMessage;
            return RedirectToHomeForms();
        }

        if (!contactSettings.FormEnabled)
        {
            ModelState.AddModelError(
                string.Empty,
                "İletişim formu şu anda mesaj kabul etmiyor.");
        }

        if (!form.HasReadKvkkNotice)
        {
            ModelState.AddModelError(
                "ContactForm.HasReadKvkkNotice",
                "KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtmelisiniz.");
        }

        var allowedSubjects = contactSettings.SubjectOptions
            .Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!allowedSubjects.Contains(form.Subject, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "ContactForm.Subject",
                "Geçerli bir başvuru türü seçmelisiniz.");
        }

        if (!ModelState.IsValid)
        {
            return View(
                "Index",
                await BuildHomePageViewModelAsync(contactForm: form));
        }

        _context.ContactMessages.Add(new ContactMessage
        {
            FullName = form.FullName.Trim(),
            Email = form.Email.Trim(),
            Subject = form.Subject.Trim(),
            Message = form.Message.Trim(),
            HasReadKvkkNotice = true,
            Status = "Yeni",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["HomeContactSuccess"] = contactSettings.SuccessMessage;
        return RedirectToHomeForms();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("public-forms")]
    [RequestFormLimits(MultipartBodyLengthLimit = MaximumReportTotalSize + 1024 * 1024)]
    public async Task<IActionResult> SubmitHomeReport(
        [Bind(Prefix = "ReportForm")] ConfidentialReportFormViewModel form,
        List<IFormFile>? reportAttachments,
        string? reportWebsite,
        string? reportReturnTarget)
    {
        if (!string.IsNullOrWhiteSpace(reportWebsite))
        {
            TempData["HomeReportSuccess"] = "Bildiriminiz başarıyla alınmıştır.";
            return RedirectToReportDestination(reportReturnTarget);
        }

        if (!ReportBranches.Contains(form.InsuranceBranch, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "ReportForm.InsuranceBranch",
                "Geçerli bir sigorta branşı seçmelisiniz.");
        }

        if (!ReportCategories.Contains(form.Category, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                "ReportForm.Category",
                "Geçerli bir ihbar kategorisi seçmelisiniz.");
        }

        if (form.IncidentDate.HasValue && form.IncidentDate.Value.Date > DateTime.Today)
        {
            ModelState.AddModelError(
                "ReportForm.IncidentDate",
                "Olay tarihi bugünden ileri bir tarih olamaz.");
        }

        if (form.WantsContact)
        {
            if (string.IsNullOrWhiteSpace(form.ReporterName))
            {
                ModelState.AddModelError(
                    "ReportForm.ReporterName",
                    "İletişim talep ediyorsanız adınızı yazmalısınız.");
            }

            if (string.IsNullOrWhiteSpace(form.ReporterEmail) &&
                string.IsNullOrWhiteSpace(form.ReporterPhone))
            {
                ModelState.AddModelError(
                    "ReportForm.ReporterEmail",
                    "E-posta veya telefon bilgilerinden en az birini yazmalısınız.");
            }
        }
        else
        {
            form.ReporterName = null;
            form.ReporterEmail = null;
            form.ReporterPhone = null;
        }

        var files = reportAttachments?
            .Where(file => file.Length > 0)
            .ToList() ?? new List<IFormFile>();

        ValidateReportFiles(files);

        if (!ModelState.IsValid)
        {
            if (IsStandaloneReport(reportReturnTarget))
            {
                return View(
                    "~/Views/Report/Index.cshtml",
                    new ConfidentialReportPageViewModel { ReportForm = form });
            }

            return View(
                "Index",
                await BuildHomePageViewModelAsync(reportForm: form));
        }

        var report = new ConfidentialReport
        {
            InsuranceBranch = form.InsuranceBranch.Trim(),
            Category = form.Category.Trim(),
            City = Clean(form.City),
            Location = Clean(form.Location),
            IncidentDate = form.IncidentDate?.Date,
            ReferenceInformation = Clean(form.ReferenceInformation),
            Details = form.Details.Trim(),
            WantsContact = form.WantsContact,
            ReporterName = Clean(form.ReporterName),
            ReporterEmail = Clean(form.ReporterEmail),
            ReporterPhone = Clean(form.ReporterPhone),
            Status = "Yeni",
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var file in files)
        {
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            report.Attachments.Add(new ConfidentialReportAttachment
            {
                OriginalFileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                FileSize = file.Length,
                Content = stream.ToArray()
            });
        }

        _context.ConfidentialReports.Add(report);
        await _context.SaveChangesAsync();

        TempData["HomeReportSuccess"] =
            "Bildiriminiz alındı. Kayıt yalnızca yetkili inceleme birimi tarafından değerlendirilecektir.";
        return RedirectToReportDestination(reportReturnTarget);
    }

    private static bool IsStandaloneReport(string? reportReturnTarget) =>
        string.Equals(reportReturnTarget, "report-page", StringComparison.Ordinal);

    private IActionResult RedirectToReportDestination(string? reportReturnTarget)
    {
        return IsStandaloneReport(reportReturnTarget)
            ? RedirectToAction(
                "Index",
                "Report",
                new { submitted = "1" },
                "reportSubmissionStatus")
            : RedirectToHomeForms();
    }

    private async Task<HomePageViewModel> BuildHomePageViewModelAsync(
        HomeContactFormViewModel? contactForm = null,
        ConfidentialReportFormViewModel? reportForm = null)
    {
        var sliders = await _context.Sliders
            .Where(slider =>
                slider.IsActive &&
                !slider.IsDeleted)
            .OrderBy(slider => slider.Order)
            .ToListAsync();

        var latestNews = await _context.News
            .AsNoTracking()
            .Where(news =>
                news.IsActive &&
                !news.IsAnnouncement)
            .OrderByDescending(news => news.CreatedAt)
            .Take(3)
            .ToListAsync();

        var upcomingEvents = await _context.AssociationEvents
            .AsNoTracking()
            .Where(associationEvent => associationEvent.IsActive)
            .OrderByDescending(associationEvent => associationEvent.IsFeatured)
            .ThenByDescending(associationEvent => associationEvent.StartDate)
            .ToListAsync();

        var relatedInstitutions = await _context.RelatedInstitutions
            .AsNoTracking()
            .Where(institution => institution.IsActive)
            .OrderBy(institution => institution.DisplayOrder)
            .ThenBy(institution => institution.Name)
            .ToListAsync();

        var president = await _context.BoardMembers
            .AsNoTracking()
            .Where(member =>
                member.Position == BoardPosition.President &&
                member.IsActive &&
                member.IsPublished)
            .OrderBy(member => member.DisplayOrder)
            .ThenBy(member => member.Id)
            .FirstOrDefaultAsync();

        ViewBag.Sliders = sliders;
        ViewBag.RelatedInstitutions = relatedInstitutions;
        ViewBag.President = president;
        ViewBag.HomeMissionSettings = await GetHomeMissionSettingsAsync();
        ViewBag.ContactSettings = await GetContactSettingsAsync();
        return new HomePageViewModel
        {
            UpcomingEvents = upcomingEvents,
            LatestNews = latestNews,
            ContactForm = contactForm ?? new HomeContactFormViewModel(),
            ReportForm = reportForm ?? new ConfidentialReportFormViewModel()
        };
    }

    public async Task<IActionResult> Privacy()
    {
        return View(await GetContactSettingsAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Volunteer()
    {
        return View(new VolunteerPageViewModel { Settings = await GetVolunteerSettingsAsync() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Volunteer(VolunteerPageViewModel model)
    {
        var settings = await GetVolunteerSettingsAsync();
        model.Settings = settings;

        if (!settings.FormEnabled)
        {
            ModelState.AddModelError(string.Empty, "Gönüllü başvuru formu şu anda başvuru kabul etmiyor.");
        }

        if (!model.HasReadKvkkNotice)
        {
            ModelState.AddModelError(nameof(model.HasReadKvkkNotice), "KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtmelisiniz.");
        }

        if (string.IsNullOrWhiteSpace(model.Application.ExpertiseArea))
        {
            ModelState.AddModelError("Application.ExpertiseArea", "Katkı alanı seçmelisiniz.");
        }

        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            TempData["SuccessMessage"] = settings.SuccessMessage;
            return RedirectToAction(nameof(Volunteer));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var volunteer = new Volunteer
        {
            FullName = model.Application.FullName.Trim(),
            Email = model.Application.Email.Trim(),
            Phone = model.Application.Phone.Trim(),
            ExpertiseArea = model.Application.ExpertiseArea.Trim(),
            Notes = model.Application.Notes?.Trim() ?? string.Empty,
            HasReadKvkkNotice = true,
            CreatedDate = DateTime.Now
        };
        _context.Volunteers.Add(volunteer);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = settings.SuccessMessage;
        return RedirectToAction(nameof(Volunteer));
    }

    [HttpGet]
    public IActionResult Donate()
    {
        return View(new Donation());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("public-forms")]
    public async Task<IActionResult> Donate(
        [Bind("DonorName,Email,Phone,Amount,Description,HasReadKvkkNotice,HasReadDonationInformation")] Donation donation)
    {
        donation.PaymentMethod = "PayTR";
        donation.PaymentStatus = "Ödeme alınmadı";

        // Bu alanlar ziyaretçiden alınmaz; sunucu tarafından güvenli
        // değerlerle doldurulduğu için bağlama aşamasındaki hatalar temizlenir.
        ModelState.Remove(nameof(donation.PaymentMethod));
        ModelState.Remove(nameof(donation.PaymentStatus));

        if (!donation.HasReadKvkkNotice)
        {
            ModelState.AddModelError(
                nameof(donation.HasReadKvkkNotice),
                "Bağış formuna devam etmek için KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtiniz.");
        }

        if (!donation.HasReadDonationInformation)
        {
            ModelState.AddModelError(
                nameof(donation.HasReadDonationInformation),
                "Bağış ve ödeme bilgilendirmesini okuduğunuzu onaylayınız.");
        }

        if (donation.Description?.Length > 2000)
        {
            ModelState.AddModelError(
                nameof(donation.Description),
                "Açıklama en fazla 2.000 karakter olabilir.");
        }

        if (!ModelState.IsValid)
        {
            return View(donation);
        }

        donation.DonorName = donation.DonorName.Trim();
        donation.Email = donation.Email.Trim();
        donation.Phone = donation.Phone.Trim();
        donation.Description = string.IsNullOrWhiteSpace(donation.Description)
            ? null
            : donation.Description.Trim();
        donation.DonationDate = DateTime.Now;
        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Bağış bildiriminiz kaydedilmiştir. Bu aşamada form üzerinden ödeme alınmamaktadır.";
        return RedirectToAction(nameof(Donate));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private async Task<VolunteerSettings> GetVolunteerSettingsAsync()
    {
        var json = await (
            from page in _context.CorporatePages.AsNoTracking()
            join section in _context.CorporateContentSections.AsNoTracking()
                on page.Id equals section.CorporatePageId
            where page.PageKey == VolunteerSeedData.PageKey &&
                  section.SectionKey == VolunteerSeedData.SettingsKey
            select section.ContentHtml)
            .SingleOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(json)) return new VolunteerSettings();
        try { return JsonSerializer.Deserialize<VolunteerSettings>(json) ?? new VolunteerSettings(); }
        catch (JsonException) { return new VolunteerSettings(); }
    }

    private async Task<HomeMissionSettings> GetHomeMissionSettingsAsync()
    {
        var json = await (
            from page in _context.CorporatePages.AsNoTracking()
            join section in _context.CorporateContentSections.AsNoTracking()
                on page.Id equals section.CorporatePageId
            where page.PageKey == HomeMissionSeedData.PageKey &&
                  section.SectionKey == HomeMissionSeedData.SettingsKey
            select section.ContentHtml)
            .SingleOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return new HomeMissionSettings();
        }

        try
        {
            return JsonSerializer.Deserialize<HomeMissionSettings>(json) ??
                   new HomeMissionSettings();
        }
        catch (JsonException)
        {
            return new HomeMissionSettings();
        }
    }

    private async Task<ContactSettings> GetContactSettingsAsync()
    {
        var json = await (
            from page in _context.CorporatePages.AsNoTracking()
            join section in _context.CorporateContentSections.AsNoTracking()
                on page.Id equals section.CorporatePageId
            where page.PageKey == ContactAndFaqSeedData.ContactPageKey &&
                  section.SectionKey == ContactAndFaqSeedData.ContactSettingsKey
            select section.ContentHtml)
            .SingleOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(json)) return new ContactSettings();

        try
        {
            return JsonSerializer.Deserialize<ContactSettings>(json) ??
                   new ContactSettings();
        }
        catch (JsonException)
        {
            return new ContactSettings();
        }
    }

    private void ValidateReportFiles(IReadOnlyList<IFormFile> files)
    {
        if (files.Count > MaximumReportFileCount)
        {
            ModelState.AddModelError(
                "ReportAttachments",
                $"En fazla {MaximumReportFileCount} dosya ekleyebilirsiniz.");
        }

        if (files.Sum(file => file.Length) > MaximumReportTotalSize)
        {
            ModelState.AddModelError(
                "ReportAttachments",
                "Eklenen dosyaların toplam boyutu 20 MB'ı geçemez.");
        }

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName);
            if (file.Length > MaximumReportFileSize)
            {
                ModelState.AddModelError(
                    "ReportAttachments",
                    $"{Path.GetFileName(file.FileName)} dosyası 10 MB sınırını aşıyor.");
            }

            if (!AllowedReportExtensions.Contains(extension) ||
                !AllowedReportContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(
                    "ReportAttachments",
                    "Yalnızca PDF, JPG ve PNG dosyaları yüklenebilir.");
            }
        }
    }

    private IActionResult RedirectToHomeForms() => RedirectToAction(
        nameof(Index),
        "Home",
        routeValues: null,
        fragment: "homeContactReport");

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
