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
[RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.View)]
public class NewsManagementController : Controller
{
    private const string UploadFolder = "news";
    private readonly ApplicationDbContext _context;
    private readonly AdminImageStorage _imageStorage;

    public NewsManagementController(ApplicationDbContext context, AdminImageStorage imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.News.AsNoTracking()
            .Where(item => !item.Title.StartsWith("__"))
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Add)]
    public IActionResult Create() => View(new NewsFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Add)]
    public async Task<IActionResult> Create(NewsFormViewModel model)
    {
        ValidateImage(model, required: true);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var imageUrl = await _imageStorage.SaveAsync(model.ImageFile!, UploadFolder);
        var item = new News
        {
            Title = model.Title.Trim(),
            Summary = model.Summary.Trim(),
            Content = model.Content.Trim(),
            ImageUrl = imageUrl,
            CreatedAt = model.PublishedAt,
            IsAnnouncement = model.IsAnnouncement,
            IsActive = model.IsActive
        };

        _context.News.Add(item);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = model.IsAnnouncement ? "Duyuru başarıyla eklendi." : "Haber başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.News.AsNoTracking()
            .SingleOrDefaultAsync(news => news.Id == id && !news.Title.StartsWith("__"));
        return item == null ? NotFound() : View(ToForm(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, NewsFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var item = await _context.News.SingleOrDefaultAsync(news => news.Id == id && !news.Title.StartsWith("__"));
        if (item == null) return NotFound();

        ValidateImage(model, required: false);
        if (!ModelState.IsValid)
        {
            model.ExistingImageUrl = item.ImageUrl;
            return View(model);
        }

        string? newImageUrl = null;
        var oldImageUrl = item.ImageUrl;
        if (model.ImageFile is { Length: > 0 })
        {
            newImageUrl = await _imageStorage.SaveAsync(model.ImageFile, UploadFolder);
            item.ImageUrl = newImageUrl;
        }

        item.Title = model.Title.Trim();
        item.Summary = model.Summary.Trim();
        item.Content = model.Content.Trim();
        item.CreatedAt = model.PublishedAt;
        item.IsAnnouncement = model.IsAnnouncement;
        item.IsActive = model.IsActive;

        await _context.SaveChangesAsync();
        if (newImageUrl != null) _imageStorage.DeleteIfManaged(oldImageUrl, UploadFolder);

        TempData["SuccessMessage"] = "Haber veya duyuru başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var item = await _context.News.SingleOrDefaultAsync(news => news.Id == id && !news.Title.StartsWith("__"));
        if (item == null) return NotFound();
        item.IsActive = !item.IsActive;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = item.IsActive ? "İçerik yayına alındı." : "İçerik yayından kaldırıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.News.SingleOrDefaultAsync(news => news.Id == id && !news.Title.StartsWith("__"));
        if (item == null) return NotFound();
        var imageUrl = item.ImageUrl;
        _context.News.Remove(item);
        await _context.SaveChangesAsync();
        _imageStorage.DeleteIfManaged(imageUrl, UploadFolder);
        TempData["SuccessMessage"] = "Haber veya duyuru silindi.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateImage(NewsFormViewModel model, bool required)
    {
        var error = _imageStorage.Validate(model.ImageFile, required && string.IsNullOrWhiteSpace(model.ExistingImageUrl));
        if (error != null) ModelState.AddModelError(nameof(model.ImageFile), error);
    }

    private static NewsFormViewModel ToForm(News item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Summary = item.Summary,
        Content = item.Content,
        PublishedAt = item.CreatedAt,
        IsAnnouncement = item.IsAnnouncement,
        IsActive = item.IsActive,
        ExistingImageUrl = item.ImageUrl
    };
}
