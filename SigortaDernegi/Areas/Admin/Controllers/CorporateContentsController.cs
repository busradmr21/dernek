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
public class CorporateContentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CorporateContentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var pages = await _context.CorporatePages
            .AsNoTracking()
            .Where(page => !page.PageKey.StartsWith("__"))
            .OrderBy(page => page.DisplayOrder)
            .ThenBy(page => page.Id)
            .ToListAsync();

        return View(pages);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var page = await _context.CorporatePages
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        return page == null ? NotFound() : View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,PageKey,MenuTitle,HeroEyebrow,HeroTitle,HeroDescription,IsActive,DisplayOrder")]
        CorporatePage model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var page = await _context.CorporatePages.SingleOrDefaultAsync(item => item.Id == id);
        if (page == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        page.MenuTitle = model.MenuTitle.Trim();
        page.HeroEyebrow = NormalizeOptionalValue(model.HeroEyebrow);
        page.HeroTitle = model.HeroTitle.Trim();
        page.HeroDescription = NormalizeOptionalValue(model.HeroDescription);
        page.IsActive = model.IsActive;
        page.DisplayOrder = model.DisplayOrder;
        page.UpdatedAt = DateTime.UtcNow;
        page.UpdatedBy = User.Identity?.Name;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Kurumsal sayfa başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sections(int pageId)
    {
        var page = await _context.CorporatePages
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == pageId);

        if (page == null)
        {
            return NotFound();
        }

        var sections = await _context.CorporateContentSections
            .AsNoTracking()
            .Where(section =>
                section.CorporatePageId == pageId &&
                !section.SectionKey.StartsWith("__"))
            .OrderBy(section => section.DisplayOrder)
            .ThenBy(section => section.Id)
            .ToListAsync();

        return View(new CorporateSectionsViewModel
        {
            Page = page,
            Sections = sections
        });
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> CreateSection(int pageId)
    {
        var page = await _context.CorporatePages
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == pageId);

        if (page == null)
        {
            return NotFound();
        }

        ViewBag.PageTitle = page.MenuTitle;

        var nextOrder = await _context.CorporateContentSections
            .Where(section => section.CorporatePageId == pageId)
            .Select(section => (int?)section.DisplayOrder)
            .MaxAsync() ?? 0;

        return View(new CorporateContentSection
        {
            CorporatePageId = pageId,
            DisplayOrder = nextOrder + 1,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> CreateSection(
        [Bind("CorporatePageId,Eyebrow,Title,ContentHtml,ImageUrl,DisplayOrder,IsActive")]
        CorporateContentSection model)
    {
        ModelState.Remove(nameof(CorporateContentSection.SectionKey));

        var pageExists = await _context.CorporatePages
            .AnyAsync(page => page.Id == model.CorporatePageId);

        if (!pageExists)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.PageTitle = await _context.CorporatePages
                .Where(page => page.Id == model.CorporatePageId)
                .Select(page => page.MenuTitle)
                .SingleAsync();
            return View(model);
        }

        model.SectionKey = $"section-{Guid.NewGuid():N}";
        model.Eyebrow = NormalizeOptionalValue(model.Eyebrow);
        model.Title = model.Title.Trim();
        model.ContentHtml = NormalizeOptionalValue(model.ContentHtml);
        model.ImageUrl = NormalizeOptionalValue(model.ImageUrl);
        model.UpdatedAt = DateTime.UtcNow;
        model.UpdatedBy = User.Identity?.Name;

        _context.CorporateContentSections.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İçerik bölümü başarıyla eklendi.";
        return RedirectToAction(nameof(Sections), new { pageId = model.CorporatePageId });
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> EditSection(int id)
    {
        var section = await _context.CorporateContentSections
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        if (section == null)
        {
            return NotFound();
        }

        ViewBag.PageTitle = await _context.CorporatePages
            .Where(page => page.Id == section.CorporatePageId)
            .Select(page => page.MenuTitle)
            .SingleAsync();

        return View(section);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> EditSection(
        int id,
        [Bind("Id,CorporatePageId,Eyebrow,Title,ContentHtml,ImageUrl,DisplayOrder,IsActive")]
        CorporateContentSection model)
    {
        ModelState.Remove(nameof(CorporateContentSection.SectionKey));

        if (id != model.Id)
        {
            return BadRequest();
        }

        var section = await _context.CorporateContentSections
            .SingleOrDefaultAsync(item => item.Id == id);

        if (section == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.PageTitle = await _context.CorporatePages
                .Where(page => page.Id == section.CorporatePageId)
                .Select(page => page.MenuTitle)
                .SingleAsync();
            return View(model);
        }

        section.Eyebrow = NormalizeOptionalValue(model.Eyebrow);
        section.Title = model.Title.Trim();
        section.ContentHtml = NormalizeOptionalValue(model.ContentHtml);
        section.ImageUrl = NormalizeOptionalValue(model.ImageUrl);
        section.DisplayOrder = model.DisplayOrder;
        section.IsActive = model.IsActive;
        section.UpdatedAt = DateTime.UtcNow;
        section.UpdatedBy = User.Identity?.Name;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İçerik bölümü başarıyla güncellendi.";
        return RedirectToAction(nameof(Sections), new { pageId = section.CorporatePageId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> DeleteSection(int id)
    {
        var section = await _context.CorporateContentSections
            .SingleOrDefaultAsync(item => item.Id == id);

        if (section == null)
        {
            return NotFound();
        }

        var pageId = section.CorporatePageId;
        _context.CorporateContentSections.Remove(section);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "İçerik bölümü silindi.";
        return RedirectToAction(nameof(Sections), new { pageId });
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
