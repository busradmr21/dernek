using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class MultimediaSeedData
{
    public const string PageKey = "__multimedia";
    private const string PressMarkerKey = "__multimedia_seed_complete";
    private const string GalleryMarkerKey = "__multimedia_gallery_seed_complete";

    private static readonly (string Key, string ImageUrl, string Title, string Description)[] GalleryItems =
    [
        ("gallery-arac-inceleme", "/images/multimedia/arac-inceleme.jpg", "Araç ve Hasar İncelemesi", "Hasar araştırmalarında aracın mevcut durumunun yerinde incelenmesi ve bulguların kayıt altına alınması."),
        ("gallery-saha-kayit", "/images/multimedia/saha-kayit.jpg", "Saha Kayıt Süreci", "Saha çalışmaları sırasında beyanların, belgelerin ve inceleme notlarının düzenli biçimde kaydedilmesi."),
        ("gallery-belge-analizi", "/images/multimedia/belge-analizi.jpg", "Belge ve Veri Analizi", "Dosya içeriğinin tutarlılık, doğrulanabilirlik ve sigorta araştırması standartları açısından değerlendirilmesi."),
        ("gallery-mesleki-egitim", "/images/multimedia/mesleki-egitim.jpg", "Mesleki Eğitim", "Araştırmacı ve uzmanların güncel mevzuat, etik ilkeler ve inceleme yöntemleri üzerine bilgi paylaşımı."),
        ("gallery-cevrimici-toplanti", "/images/multimedia/cevrimici-toplanti.jpg", "Çevrim İçi Çalışmalar", "Farklı şehirlerdeki uzmanların webinarlar ve çevrim içi toplantılarla ortak çalışma yürütmesi."),
        ("gallery-kurul-calismasi", "/images/multimedia/kurul-calismasi.jpg", "Kurul ve Komisyon Çalışmaları", "Mesleki standartların geliştirilmesi için yönetim, kurul ve çalışma gruplarının bir araya gelmesi.")
    ];

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var page = await context.CorporatePages.SingleOrDefaultAsync(item => item.PageKey == PageKey);
        if (page == null)
        {
            page = new CorporatePage
            {
                PageKey = PageKey,
                MenuTitle = "Multimedya",
                HeroTitle = "Basında Biz",
                IsActive = false,
                DisplayOrder = 999
            };
            context.CorporatePages.Add(page);
            await context.SaveChangesAsync();
        }

        var existingKeys = await context.CorporateContentSections
            .Where(item => item.CorporatePageId == page.Id)
            .Select(item => item.SectionKey)
            .ToListAsync();

        if (!existingKeys.Contains(PressMarkerKey, StringComparer.OrdinalIgnoreCase))
        {
            var order = 1;
            foreach (var article in PressContent.Items)
            {
                if (!existingKeys.Contains(article.Slug, StringComparer.OrdinalIgnoreCase))
                {
                    var source = article.Sources.FirstOrDefault();
                    var payload = new MultimediaPayload
                    {
                        ContentType = "Press",
                        Summary = article.Summary,
                        Content = string.Join("\n\n", article.Paragraphs),
                        SourceName = source?.Name,
                        SourceUrl = source?.Url
                    };

                    context.CorporateContentSections.Add(new CorporateContentSection
                    {
                        CorporatePageId = page.Id,
                        SectionKey = article.Slug,
                        Eyebrow = article.Category,
                        Title = article.Title,
                        ContentHtml = JsonSerializer.Serialize(payload),
                        ImageUrl = article.ImageUrl,
                        DisplayOrder = order,
                        IsActive = true,
                        UpdatedAt = article.PublishedAt,
                        UpdatedBy = "System"
                    });
                }

                order++;
            }

            context.CorporateContentSections.Add(CreateMarker(page.Id, PressMarkerKey, "Basın içerikleri başlangıç kaydı"));
        }

        if (!existingKeys.Contains(GalleryMarkerKey, StringComparer.OrdinalIgnoreCase))
        {
            var order = 1;
            foreach (var galleryItem in GalleryItems)
            {
                if (!existingKeys.Contains(galleryItem.Key, StringComparer.OrdinalIgnoreCase))
                {
                    var payload = new MultimediaPayload
                    {
                        ContentType = "Gallery",
                        Summary = galleryItem.Description
                    };

                    context.CorporateContentSections.Add(new CorporateContentSection
                    {
                        CorporatePageId = page.Id,
                        SectionKey = galleryItem.Key,
                        Eyebrow = "Foto Galeri",
                        Title = galleryItem.Title,
                        ContentHtml = JsonSerializer.Serialize(payload),
                        ImageUrl = galleryItem.ImageUrl,
                        DisplayOrder = order,
                        IsActive = true,
                        UpdatedAt = new DateTime(2026, 1, 1).AddMinutes(-order),
                        UpdatedBy = "System"
                    });
                }

                order++;
            }

            context.CorporateContentSections.Add(CreateMarker(page.Id, GalleryMarkerKey, "Foto galeri başlangıç kaydı"));
        }

        await context.SaveChangesAsync();
    }

    private static CorporateContentSection CreateMarker(int pageId, string key, string title) => new()
    {
        CorporatePageId = pageId,
        SectionKey = key,
        Title = title,
        DisplayOrder = -1,
        IsActive = false,
        UpdatedBy = "System"
    };
}
