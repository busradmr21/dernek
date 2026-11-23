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
public class EventsManagementController : Controller
{
    private const string UploadFolder = "events";
    private readonly ApplicationDbContext _context;
    private readonly AdminImageStorage _imageStorage;

    public EventsManagementController(ApplicationDbContext context, AdminImageStorage imageStorage)
    {
        _context = context;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.AssociationEvents.AsNoTracking()
            .Where(item => !item.Title.StartsWith("__"))
            .OrderByDescending(item => item.StartDate)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Add)]
    public IActionResult Create() => View(new EventFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Add)]
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        Validate(model, requiredImage: true);
        if (!ModelState.IsValid) return View(model);

        var imageUrl = await _imageStorage.SaveAsync(model.ImageFile!, UploadFolder);
        var item = new AssociationEvent();
        Apply(item, model);
        item.ImageUrl = imageUrl;
        item.CreatedAt = DateTime.UtcNow;
        _context.AssociationEvents.Add(item);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Etkinlik başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.AssociationEvents.AsNoTracking()
            .SingleOrDefaultAsync(eventItem => eventItem.Id == id && !eventItem.Title.StartsWith("__"));
        return item == null ? NotFound() : View(ToForm(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, EventFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var item = await _context.AssociationEvents
            .SingleOrDefaultAsync(eventItem => eventItem.Id == id && !eventItem.Title.StartsWith("__"));
        if (item == null) return NotFound();

        Validate(model, requiredImage: false);
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

        Apply(item, model);
        await _context.SaveChangesAsync();
        if (newImageUrl != null) _imageStorage.DeleteIfManaged(oldImageUrl, UploadFolder);
        TempData["SuccessMessage"] = "Etkinlik başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var item = await _context.AssociationEvents
            .SingleOrDefaultAsync(eventItem => eventItem.Id == id && !eventItem.Title.StartsWith("__"));
        if (item == null) return NotFound();
        item.IsActive = !item.IsActive;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = item.IsActive ? "Etkinlik yayına alındı." : "Etkinlik yayından kaldırıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.HomeUpdates, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.AssociationEvents
            .SingleOrDefaultAsync(eventItem => eventItem.Id == id && !eventItem.Title.StartsWith("__"));
        if (item == null) return NotFound();
        var imageUrl = item.ImageUrl;
        _context.AssociationEvents.Remove(item);
        await _context.SaveChangesAsync();
        _imageStorage.DeleteIfManaged(imageUrl, UploadFolder);
        TempData["SuccessMessage"] = "Etkinlik silindi.";
        return RedirectToAction(nameof(Index));
    }

    private void Validate(EventFormViewModel model, bool requiredImage)
    {
        if (model.EndDate.HasValue && model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "Bitiş tarihi başlangıç tarihinden önce olamaz.");
        }

        var error = _imageStorage.Validate(model.ImageFile, requiredImage && string.IsNullOrWhiteSpace(model.ExistingImageUrl));
        if (error != null) ModelState.AddModelError(nameof(model.ImageFile), error);
    }

    private static void Apply(AssociationEvent item, EventFormViewModel model)
    {
        item.Title = model.Title.Trim();
        item.Summary = model.Summary.Trim();
        item.Description = Normalize(model.Description);
        item.StartDate = model.StartDate;
        item.EndDate = model.EndDate;
        item.EventType = Normalize(model.EventType);
        item.Location = Normalize(model.Location);
        item.RegistrationUrl = Normalize(model.RegistrationUrl);
        item.IsOnline = model.IsOnline;
        item.IsFeatured = model.IsFeatured;
        item.IsActive = model.IsActive;
    }

    private static EventFormViewModel ToForm(AssociationEvent item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Summary = item.Summary,
        Description = item.Description,
        StartDate = item.StartDate,
        EndDate = item.EndDate,
        EventType = item.EventType,
        Location = item.Location,
        RegistrationUrl = item.RegistrationUrl,
        IsOnline = item.IsOnline,
        IsFeatured = item.IsFeatured,
        IsActive = item.IsActive,
        ExistingImageUrl = item.ImageUrl
    };

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
