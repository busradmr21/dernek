using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.ViewComponents;

public class FooterContactViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public FooterContactViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var json = await (
            from page in _context.CorporatePages.AsNoTracking()
            join section in _context.CorporateContentSections.AsNoTracking()
                on page.Id equals section.CorporatePageId
            where page.PageKey == ContactAndFaqSeedData.ContactPageKey &&
                  section.SectionKey == ContactAndFaqSeedData.ContactSettingsKey
            select section.ContentHtml)
            .SingleOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return View(new ContactSettings());
        }

        try
        {
            return View(
                JsonSerializer.Deserialize<ContactSettings>(json) ??
                new ContactSettings());
        }
        catch (JsonException)
        {
            return View(new ContactSettings());
        }
    }
}
