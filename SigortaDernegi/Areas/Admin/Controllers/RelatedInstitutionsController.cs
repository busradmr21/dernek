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
public class RelatedInstitutionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RelatedInstitutionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var institutions = await _context.RelatedInstitutions
            .AsNoTracking()
            .OrderBy(institution => institution.DisplayOrder)
            .ThenBy(institution => institution.Name)
            .ToListAsync();

        return View(institutions);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create()
    {
        var nextOrder = await _context.RelatedInstitutions.AnyAsync()
            ? await _context.RelatedInstitutions.MaxAsync(item => item.DisplayOrder) + 1
            : 1;

        return View(new RelatedInstitutionFormViewModel
        {
            DisplayOrder = nextOrder,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create(RelatedInstitutionFormViewModel model)
    {
        ValidateWebsiteUrl(model.WebsiteUrl);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var institution = new RelatedInstitution
        {
            Abbreviation = model.Abbreviation.Trim(),
            Name = model.Name.Trim(),
            WebsiteUrl = model.WebsiteUrl.Trim(),
            DisplayOrder = model.DisplayOrder,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.RelatedInstitutions.Add(institution);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kurum kartı başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var institution = await _context.RelatedInstitutions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        if (institution == null)
        {
            return NotFound();
        }

        return View(new RelatedInstitutionFormViewModel
        {
            Id = institution.Id,
            Abbreviation = institution.Abbreviation,
            Name = institution.Name,
            WebsiteUrl = institution.WebsiteUrl,
            DisplayOrder = institution.DisplayOrder,
            IsActive = institution.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, RelatedInstitutionFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidateWebsiteUrl(model.WebsiteUrl);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var institution = await _context.RelatedInstitutions
            .SingleOrDefaultAsync(item => item.Id == id);

        if (institution == null)
        {
            return NotFound();
        }

        institution.Abbreviation = model.Abbreviation.Trim();
        institution.Name = model.Name.Trim();
        institution.WebsiteUrl = model.WebsiteUrl.Trim();
        institution.DisplayOrder = model.DisplayOrder;
        institution.IsActive = model.IsActive;
        institution.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kurum kartı başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var institution = await _context.RelatedInstitutions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        return institution == null ? NotFound() : View(institution);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var institution = await _context.RelatedInstitutions
            .SingleOrDefaultAsync(item => item.Id == id);

        if (institution == null)
        {
            return NotFound();
        }

        _context.RelatedInstitutions.Remove(institution);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kurum kartı başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var institution = await _context.RelatedInstitutions
            .SingleOrDefaultAsync(item => item.Id == id);

        if (institution == null)
        {
            return NotFound();
        }

        institution.IsActive = !institution.IsActive;
        institution.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = institution.IsActive
            ? "Kurum kartı ana sayfada yayına alındı."
            : "Kurum kartı ana sayfadan kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }

    private void ValidateWebsiteUrl(string? websiteUrl)
    {
        if (string.IsNullOrWhiteSpace(websiteUrl))
        {
            return;
        }

        if (!Uri.TryCreate(websiteUrl, UriKind.Absolute, out var parsedUrl) ||
            (parsedUrl.Scheme != Uri.UriSchemeHttp &&
             parsedUrl.Scheme != Uri.UriSchemeHttps))
        {
            ModelState.AddModelError(
                nameof(RelatedInstitutionFormViewModel.WebsiteUrl),
                "Bağlantı http veya https ile başlayan geçerli bir adres olmalıdır.");
        }
    }
}
