using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class VolunteerSeedData
{
    public const string PageKey = "__volunteer-settings";
    public const string SettingsKey = "settings";
    public const string ApplicationPrefix = "volunteer-application-";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var page = await context.CorporatePages.SingleOrDefaultAsync(item => item.PageKey == PageKey);
        if (page == null)
        {
            page = new CorporatePage
            {
                PageKey = PageKey,
                MenuTitle = "Gönüllülük Yönetimi",
                HeroTitle = "Gönüllü Ol",
                IsActive = false,
                DisplayOrder = 996,
                UpdatedBy = "System"
            };
            context.CorporatePages.Add(page);
            await context.SaveChangesAsync();
        }

        if (!await context.CorporateContentSections.AnyAsync(item =>
                item.CorporatePageId == page.Id && item.SectionKey == SettingsKey))
        {
            context.CorporateContentSections.Add(new CorporateContentSection
            {
                CorporatePageId = page.Id,
                SectionKey = SettingsKey,
                Title = "Gönüllülük sayfası ayarları",
                ContentHtml = JsonSerializer.Serialize(new VolunteerSettings()),
                IsActive = true,
                UpdatedBy = "System"
            });
            await context.SaveChangesAsync();
        }
    }
}
