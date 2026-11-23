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
[RequirePermission(ManagementArea.SiteContent, PermissionActionType.View)]
public class VolunteerSettingsController : Controller
{
    private const string RevisionPrefix = "revision-volunteer-";
    private readonly ApplicationDbContext _context;

    public VolunteerSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var current = await GetCurrentAsync(pageId.Value);
        if (current == null) return NotFound();
        return View(new VolunteerSettingsAdminViewModel
        {
            Settings = Deserialize(current.ContentHtml),
            Revisions = await GetRevisionsAsync(pageId.Value)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Save(VolunteerSettingsAdminViewModel model)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var current = await GetCurrentAsync(pageId.Value);
        if (current == null) return NotFound();

        if (!ModelState.IsValid)
        {
            model.Revisions = await GetRevisionsAsync(pageId.Value);
            return View("Index", model);
        }

        SaveRevision(pageId.Value, current.ContentHtml, "Düzenleme öncesi");
        current.ContentHtml = JsonSerializer.Serialize(model.Settings);
        current.UpdatedAt = DateTime.UtcNow;
        current.UpdatedBy = User.Identity?.Name;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Gönüllülük sayfası başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Restore(int id)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var current = await GetCurrentAsync(pageId.Value);
        var revision = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.Id == id && item.CorporatePageId == pageId && item.SectionKey.StartsWith(RevisionPrefix));
        if (current == null || revision == null) return NotFound();

        SaveRevision(pageId.Value, current.ContentHtml, "Geri yükleme öncesi");
        current.ContentHtml = revision.ContentHtml;
        current.UpdatedAt = DateTime.UtcNow;
        current.UpdatedBy = User.Identity?.Name;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Seçilen gönüllülük sayfası sürümü geri yüklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteRevision(int id)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var revision = await _context.CorporateContentSections.SingleOrDefaultAsync(item =>
            item.Id == id && item.CorporatePageId == pageId && item.SectionKey.StartsWith(RevisionPrefix));
        if (revision == null) return NotFound();
        _context.CorporateContentSections.Remove(revision);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Değişiklik geçmişi kaydı silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> GetPageIdAsync() => await _context.CorporatePages
        .Where(item => item.PageKey == VolunteerSeedData.PageKey)
        .Select(item => (int?)item.Id)
        .SingleOrDefaultAsync();

    private async Task<CorporateContentSection?> GetCurrentAsync(int pageId) => await _context.CorporateContentSections
        .SingleOrDefaultAsync(item => item.CorporatePageId == pageId && item.SectionKey == VolunteerSeedData.SettingsKey);

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

    private static VolunteerSettings Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new VolunteerSettings();
        try { return JsonSerializer.Deserialize<VolunteerSettings>(json) ?? new VolunteerSettings(); }
        catch (JsonException) { return new VolunteerSettings(); }
    }
}
