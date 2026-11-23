using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(new ContactPageViewModel { Settings = await GetSettingsAsync() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactPageViewModel model)
    {
        var settings = await GetSettingsAsync();
        model.Settings = settings;

        if (!settings.FormEnabled)
        {
            ModelState.AddModelError(string.Empty, "İletişim formu şu anda mesaj kabul etmiyor.");
        }

        if (!model.Message.HasReadKvkkNotice)
        {
            ModelState.AddModelError(
                "Message.HasReadKvkkNotice",
                "KVKK Aydınlatma Metni’ni okuyup bilgi edindiğinizi belirtmelisiniz.");
        }

        if (!model.Subjects.Contains(model.Message.Subject, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("Message.Subject", "Geçerli bir konu seçmelisiniz.");
        }

        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            TempData["SuccessMessage"] = settings.SuccessMessage;
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = new ContactMessage
        {
            FullName = model.Message.FullName.Trim(),
            Email = model.Message.Email.Trim(),
            Phone = Normalize(model.Message.Phone),
            Subject = model.Message.Subject.Trim(),
            Message = model.Message.Message.Trim(),
            HasReadKvkkNotice = true,
            Status = "Yeni",
            CreatedAt = DateTime.UtcNow
        };

        await _context.ContactMessages.AddAsync(message);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = settings.SuccessMessage;
        return RedirectToAction(nameof(Index));
    }

    private async Task<ContactSettings> GetSettingsAsync()
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
        try { return JsonSerializer.Deserialize<ContactSettings>(json) ?? new ContactSettings(); }
        catch (JsonException) { return new ContactSettings(); }
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
