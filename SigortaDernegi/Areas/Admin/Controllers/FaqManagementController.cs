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
public class FaqManagementController : Controller
{
    private readonly ApplicationDbContext _context;

    public FaqManagementController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var items = await _context.CorporateContentSections.AsNoTracking()
            .Where(item => item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"))
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Id)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create()
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();
        var nextOrder = await _context.CorporateContentSections
            .Where(item => item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"))
            .Select(item => (int?)item.DisplayOrder)
            .MaxAsync() ?? 0;
        return View(new FaqFormViewModel { DisplayOrder = nextOrder + 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Add)]
    public async Task<IActionResult> Create(FaqFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var pageId = await GetPageIdAsync();
        if (pageId == null) return NotFound();

        var item = new CorporateContentSection
        {
            CorporatePageId = pageId.Value,
            SectionKey = $"faq-{Guid.NewGuid():N}"
        };
        Apply(item, model);
        _context.CorporateContentSections.Add(item);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Soru ve yanıt başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await FindAsync(id, tracking: false);
        return item == null ? NotFound() : View(ToForm(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id, FaqFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        var item = await FindAsync(id, tracking: true);
        if (item == null) return NotFound();
        if (!ModelState.IsValid) return View(model);

        Apply(item, model);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Soru ve yanıt başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var item = await FindAsync(id, tracking: true);
        if (item == null) return NotFound();
        item.IsActive = !item.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = User.Identity?.Name;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = item.IsActive ? "Soru yayına alındı." : "Soru yayından kaldırıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(ManagementArea.SiteContent, PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await FindAsync(id, tracking: true);
        if (item == null) return NotFound();
        _context.CorporateContentSections.Remove(item);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Soru ve yanıt silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> GetPageIdAsync() => await _context.CorporatePages
        .Where(item => item.PageKey == ContactAndFaqSeedData.FaqPageKey)
        .Select(item => (int?)item.Id)
        .SingleOrDefaultAsync();

    private async Task<CorporateContentSection?> FindAsync(int id, bool tracking)
    {
        var pageId = await GetPageIdAsync();
        if (pageId == null) return null;
        var query = tracking
            ? _context.CorporateContentSections.AsQueryable()
            : _context.CorporateContentSections.AsNoTracking();
        return await query.SingleOrDefaultAsync(item =>
            item.Id == id && item.CorporatePageId == pageId && !item.SectionKey.StartsWith("__"));
    }

    private void Apply(CorporateContentSection item, FaqFormViewModel model)
    {
        item.Eyebrow = model.Category.Trim();
        item.Title = model.Question.Trim();
        item.ContentHtml = model.Answer.Trim();
        item.DisplayOrder = model.DisplayOrder;
        item.IsActive = model.IsActive;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = User.Identity?.Name;
    }

    private static FaqFormViewModel ToForm(CorporateContentSection item) => new()
    {
        Id = item.Id,
        Category = item.Eyebrow ?? "Genel",
        Question = item.Title,
        Answer = item.ContentHtml ?? string.Empty,
        DisplayOrder = item.DisplayOrder,
        IsActive = item.IsActive
    };
}
