using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers;

public class MultimediaController : Controller
{
    private readonly ApplicationDbContext _context;

    public MultimediaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await GetItemsAsync();
        return View(new MultimediaIndexViewModel
        {
            GalleryItems = items.Where(item => item.IsGallery).Select(item => item.Article).ToList(),
            PressItems = items.Where(item => !item.IsGallery).Select(item => item.Article).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> PressDetail(string slug)
    {
        var item = (await GetItemsAsync()).FirstOrDefault(item =>
            !item.IsGallery && string.Equals(item.Article.Slug, slug, StringComparison.OrdinalIgnoreCase));
        return item == null ? NotFound() : View(item.Article);
    }

    private async Task<IReadOnlyList<MultimediaRecord>> GetItemsAsync()
    {
        var pageId = await _context.CorporatePages
            .Where(page => page.PageKey == MultimediaSeedData.PageKey)
            .Select(page => (int?)page.Id)
            .SingleOrDefaultAsync();

        if (pageId == null) return Array.Empty<MultimediaRecord>();

        var sections = await _context.CorporateContentSections.AsNoTracking()
            .Where(item =>
                item.CorporatePageId == pageId &&
                item.IsActive &&
                !item.SectionKey.StartsWith("__"))
            .OrderByDescending(item => item.UpdatedAt)
            .ToListAsync();

        return sections.Select(section =>
        {
            var payload = Deserialize(section.ContentHtml);
            return new MultimediaRecord(
                ToArticle(section, payload),
                string.Equals(payload.ContentType, "Gallery", StringComparison.OrdinalIgnoreCase));
        }).ToList();
    }

    private static PressArticle ToArticle(CorporateContentSection section, MultimediaPayload payload)
    {
        var sources = string.IsNullOrWhiteSpace(payload.SourceName) || string.IsNullOrWhiteSpace(payload.SourceUrl)
            ? Array.Empty<PressSource>()
            : new[] { new PressSource { Name = payload.SourceName, Url = payload.SourceUrl } };

        return new PressArticle
        {
            Slug = section.SectionKey,
            ImageUrl = section.ImageUrl ?? string.Empty,
            Category = section.Eyebrow ?? "Basın",
            Title = section.Title,
            Summary = payload.Summary,
            PublishedAt = section.UpdatedAt,
            Paragraphs = payload.Content
                .Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            Sources = sources
        };
    }

    private static MultimediaPayload Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new MultimediaPayload();
        try { return JsonSerializer.Deserialize<MultimediaPayload>(json) ?? new MultimediaPayload(); }
        catch (JsonException) { return new MultimediaPayload { Content = json }; }
    }

    private sealed record MultimediaRecord(PressArticle Article, bool IsGallery);
}
