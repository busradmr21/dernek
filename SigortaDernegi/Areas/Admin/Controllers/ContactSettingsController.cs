using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
[RequirePermission(ManagementArea.ApplicationsAndCommunication, PermissionActionType.View)]
public class ContactSettingsController : Controller
{
    private const string RevisionPrefix = "revision-contact-";
    private readonly ApplicationDbContext _context;

    public ContactSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var page = await GetPageAsync();
        if (page == null) return NotFound();

        var current = await GetCurrentAsync(page.Id);
        if (current == null) return NotFound();

        return View(new ContactSettingsAdminViewModel
        {
            Settings = Deserialize(current.ContentHtml),
            Revisions = await GetRevisionsAsync(page.Id)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.ApplicationsAndCommunication, PermissionActionType.Edit)]
    public async Task<IActionResult> Save(ContactSettingsAdminViewModel model)
    {
        var page = await GetPageAsync();
        if (page == null) return NotFound();
        var current = await GetCurrentAsync(page.Id);
        if (current == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Revisions = await GetRevisionsAsync(page.Id);
            return View("Index", model);
        }

        SaveRevision(page.Id, current.ContentHtml, "Düzenleme öncesi");
        current.ContentHtml = JsonSerializer.Serialize(model.Settings);
        current.UpdatedAt = DateTime.UtcNow;
        current.UpdatedBy = User.Identity?.Name;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İletişim bilgileri başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.ApplicationsAndCommunication, PermissionActionType.Edit)]
    public async Task<IActionResult> Restore(int id)
    {
        var page = await GetPageAsync();
        if (page == null) return NotFound();
        var current = await GetCurrentAsync(page.Id);
        var revision = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.Id == id &&
            item.CorporatePageId == page.Id &&
            item.SectionKey.StartsWith(RevisionPrefix));
        if (current == null || revision == null) return NotFound();

        SaveRevision(page.Id, current.ContentHtml, "Geri yükleme öncesi");
        current.ContentHtml = revision.ContentHtml;
        current.UpdatedAt = DateTime.UtcNow;
        current.UpdatedBy = User.Identity?.Name;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Seçilen iletişim ayarları sürümü geri yüklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.ApplicationsAndCommunication, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteRevision(int id)
    {
        var page = await GetPageAsync();
        if (page == null) return NotFound();
        var revision = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.Id == id &&
            item.CorporatePageId == page.Id &&
            item.SectionKey.StartsWith(RevisionPrefix));
        if (revision == null) return NotFound();

        _context.CorporateContentSections.Remove(revision);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Değişiklik geçmişi kaydı silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<CorporatePage?> GetPageAsync() => await _context.CorporatePages
        .SingleOrDefaultAsync(item => item.PageKey == ContactAndFaqSeedData.ContactPageKey);

    private async Task<CorporateContentSection?> GetCurrentAsync(int pageId) => await _context.CorporateContentSections
        .SingleOrDefaultAsync(item =>
            item.CorporatePageId == pageId && item.SectionKey == ContactAndFaqSeedData.ContactSettingsKey);

    private async Task<IReadOnlyList<ContactSettingsRevisionViewModel>> GetRevisionsAsync(int pageId) =>
        await _context.CorporateContentSections.AsNoTracking()
            .Where(item => item.CorporatePageId == pageId && item.SectionKey.StartsWith(RevisionPrefix))
            .OrderByDescending(item => item.UpdatedAt)
            .Take(20)
            .Select(item => new ContactSettingsRevisionViewModel
            {
                Id = item.Id,
                ChangedAt = item.UpdatedAt,
                ChangedBy = item.UpdatedBy ?? "Yönetici",
                Summary = item.Title
            })
            .ToListAsync();

    private void SaveRevision(int pageId, string? json, string title)
    {
        _context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = pageId,
            SectionKey = $"{RevisionPrefix}{Guid.NewGuid():N}",
            Title = title,
            ContentHtml = json,
            DisplayOrder = -1,
            IsActive = false,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = User.Identity?.Name
        });
    }

    private static ContactSettings Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new ContactSettings();
        try { return JsonSerializer.Deserialize<ContactSettings>(json) ?? new ContactSettings(); }
        catch (JsonException) { return new ContactSettings(); }
    }
}
