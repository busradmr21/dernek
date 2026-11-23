using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.Services;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
[RequirePermission(ManagementArea.SiteContent, PermissionActionType.View)]
public class MultimediaManagementController : Controller
{
    private const string UploadFolder = "multimedia";
    private readonly ApplicationDbContext _context;
    private readonly AdminImageStorage _imageStorage;

    public MultimediaManagementController(ApplicationDbContext context, AdminImageStorage imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index()
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();

        var sections = await _context.CorporateContentSections.AsNoTracking()
            .Where(item => item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"))
            .OrderByDescending(item => item.UpdatedAt)
            .ToListAsync();

        return View(sections.Select(ToForm).ToList());
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public IActionResult Create() => View(new MultimediaFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create(MultimediaFormViewModel model)
    {
        Validate(model, requiredImage: true);
        if (!ModelState.IsValid) return View(model);

        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();

        var imageUrl = await _imageStorage.SaveAsync(model.ImageFile!, UploadFolder);
        var section = new CorporateContentSection
        {
            CorporatePageId = pageId.Value,
            SectionKey = $"media-{Guid.NewGuid():N}",
            ImageUrl = imageUrl
        };
        Apply(section, model);
        _context.CorporateContentSections.Add(section);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Multimedya içeriği başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var section = await _context.CorporateContentSections.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id && item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"));
        return section == null ? NotFound() : View(ToForm(section));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, MultimediaFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var section = await _context.CorporateContentSections
            .SingleOrDefaultAsync(item => item.Id == id && item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"));
        if (section == null) return NotFound();

        Validate(model, requiredImage: false);
        if (!ModelState.IsValid)
        {
            model.ExistingImageUrl = section.ImageUrl;
            return View(model);
        }

        string? newImageUrl = null;
        var oldImageUrl = section.ImageUrl;
        if (model.ImageFile is { Length: > 0 })
        {
            newImageUrl = await _imageStorage.SaveAsync(model.ImageFile, UploadFolder);
            section.ImageUrl = newImageUrl;
        }

        Apply(section, model);
        await _context.SaveChangesAsync();
        if (newImageUrl != null) _imageStorage.DeleteIfManaged(oldImageUrl, UploadFolder);
        TempData["SuccessMessage"] = "Multimedya içeriği başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var section = await _context.CorporateContentSections
            .SingleOrDefaultAsync(item => item.Id == id && item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"));
        if (section == null) return NotFound();
        section.IsActive = !section.IsActive;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = section.IsActive ? "İçerik yayına alındı." : "İçerik yayından kaldırıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var section = await _context.CorporateContentSections
            .SingleOrDefaultAsync(item => item.Id == id && item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"));
        if (section == null) return NotFound();
        var imageUrl = section.ImageUrl;
        _context.CorporateContentSections.Remove(section);
        await _context.SaveChangesAsync();
        _imageStorage.DeleteIfManaged(imageUrl, UploadFolder);
        TempData["SuccessMessage"] = "Multimedya içeriği silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> GetPageIdAsync() => await _context.CorporatePages
        .Where(page => page.PageKey == MultimediaSeedData.PageKey)
        .Select(page => (int?)page.Id)
        .SingleOrDefaultAsync();

    private void Validate(MultimediaFormViewModel model, bool requiredImage)
    {
        if (string.IsNullOrWhiteSpace(model.SourceName) != string.IsNullOrWhiteSpace(model.SourceUrl))
        {
            ModelState.AddModelError(nameof(model.SourceUrl), "Kaynak adı ve bağlantısı birlikte doldurulmalıdır.");
        }

        if (!model.IsGallery && string.IsNullOrWhiteSpace(model.Content))
        {
            ModelState.AddModelError(nameof(model.Content), "Basın içeriği için detay metni zorunludur.");
        }

        var error = _imageStorage.Validate(model.ImageFile, requiredImage && string.IsNullOrWhiteSpace(model.ExistingImageUrl));
        if (error != null) ModelState.AddModelError(nameof(model.ImageFile), error);
    }

    private static void Apply(CorporateContentSection section, MultimediaFormViewModel model)
    {
        var payload = new MultimediaPayload
        {
            ContentType = model.IsGallery ? "Gallery" : "Press",
            Summary = model.Summary.Trim(),
            Content = model.Content.Trim(),
            SourceName = Normalize(model.SourceName),
            SourceUrl = Normalize(model.SourceUrl)
        };
        section.Eyebrow = model.Category.Trim();
        section.Title = model.Title.Trim();
        section.ContentHtml = JsonSerializer.Serialize(payload);
        section.IsActive = model.IsActive;
        section.UpdatedAt = model.PublishedAt;
    }

    private static MultimediaFormViewModel ToForm(CorporateContentSection section)
    {
        var payload = Deserialize(section.ContentHtml);
        return new MultimediaFormViewModel
        {
            Id = section.Id,
            Category = section.Eyebrow ?? "Basın",
            Title = section.Title,
            Summary = payload.Summary,
            Content = payload.Content,
            SourceName = payload.SourceName,
            SourceUrl = payload.SourceUrl,
            PublishedAt = section.UpdatedAt,
            IsActive = section.IsActive,
            IsGallery = string.Equals(payload.ContentType, "Gallery", StringComparison.OrdinalIgnoreCase),
            ExistingImageUrl = section.ImageUrl
        };
    }

    private static MultimediaPayload Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new MultimediaPayload();
        try { return JsonSerializer.Deserialize<MultimediaPayload>(json) ?? new MultimediaPayload(); }
        catch (JsonException) { return new MultimediaPayload { Content = json }; }
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
