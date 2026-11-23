using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class HomeMissionSeedData
{
    public const string PageKey = "__home-mission-settings";
    public const string SettingsKey = "settings";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var page = await context.CorporatePages
            .SingleOrDefaultAsync(item => item.PageKey == PageKey);

        if (page == null)
        {
            page = new CorporatePage
            {
                PageKey = PageKey,
                MenuTitle = "Ana Sayfa Misyon Alanı",
                HeroTitle = "Ana Sayfa Misyon Alanı",
                IsActive = false,
                DisplayOrder = -1
            };
            context.CorporatePages.Add(page);
            await context.SaveChangesAsync();
        }

        var exists = await context.CorporateContentSections.AnyAsync(item =>
            item.CorporatePageId == page.Id && item.SectionKey == SettingsKey);

        if (exists)
        {
            return;
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = page.Id,
            SectionKey = SettingsKey,
            Title = "Ana Sayfa Misyon Ayarları",
            ContentHtml = JsonSerializer.Serialize(new HomeMissionSettings()),
            DisplayOrder = 0,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "Sistem"
        });

        await context.SaveChangesAsync();
    }
}
