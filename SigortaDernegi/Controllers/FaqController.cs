using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers;

public class FaqController : Controller
{
    private readonly ApplicationDbContext _context;

    public FaqController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var page = await _context.CorporatePages.AsNoTracking()
            .SingleOrDefaultAsync(item => item.PageKey == ContactAndFaqSeedData.FaqPageKey);
        if (page == null) return NotFound();

        var items = await _context.CorporateContentSections.AsNoTracking()
            .Where(item =>
                item.CorporatePageId == page.Id &&
                item.IsActive &&
                !item.SectionKey.StartsWith("__"))
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Id)
            .Select(item => new FaqItemViewModel
            {
                Id = item.Id,
                Category = item.Eyebrow ?? "Genel",
                Question = item.Title,
                Answer = item.ContentHtml ?? string.Empty
            })
            .ToListAsync();

        return View(new FaqPageViewModel
        {
            HeroEyebrow = page.HeroEyebrow ?? "Merak Ettikleriniz",
            HeroTitle = page.HeroTitle,
            HeroDescription = page.HeroDescription ?? string.Empty,
            Items = items
        });
    }
}
