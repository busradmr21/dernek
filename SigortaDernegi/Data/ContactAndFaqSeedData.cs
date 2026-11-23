using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class ContactAndFaqSeedData
{
    public const string ContactPageKey = "__contact-settings";
    public const string ContactSettingsKey = "settings";
    public const string FaqPageKey = "__faq";
    private const string FaqSeedMarkerKey = "__faq_seed_complete";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var contactPage = await EnsurePageAsync(
            context,
            ContactPageKey,
            "İletişim Yönetimi",
            "İletişim Ayarları",
            997);

        if (!await context.CorporateContentSections.AnyAsync(section =>
                section.CorporatePageId == contactPage.Id && section.SectionKey == ContactSettingsKey))
        {
            context.CorporateContentSections.Add(new CorporateContentSection
            {
                CorporatePageId = contactPage.Id,
                SectionKey = ContactSettingsKey,
                Title = "İletişim sayfası ayarları",
                ContentHtml = JsonSerializer.Serialize(new ContactSettings()),
                DisplayOrder = 0,
                IsActive = true,
                UpdatedBy = "System"
            });
        }

        var faqPage = await EnsurePageAsync(
            context,
            FaqPageKey,
            "Sıkça Sorulan Sorular",
            "Sıkça Sorulan Sorular",
            998,
            "Merak Ettikleriniz",
            "Üyelik, etkinlikler ve dernek çalışmaları hakkında sık sorulan soruların yanıtlarını inceleyin.");

        var faqSeeded = await context.CorporateContentSections.AnyAsync(section =>
            section.CorporatePageId == faqPage.Id && section.SectionKey == FaqSeedMarkerKey);

        if (!faqSeeded)
        {
            var defaults = new[]
            {
                ("Üyelik", "Kimler ELSADER üyesi olabilir?", "Derneğin amaçlarını benimseyen ve üyelik koşullarını sağlayan sigorta acenteleri ve ilgili meslek mensupları üyelik başvurusu yapabilir."),
                ("Üyelik", "Üyelik başvurusu nasıl yapılır?", "Üyelik menüsündeki Online Üyelik Başvurusu sayfasından formu doldurabilirsiniz. Başvurunuz yönetim tarafından incelendikten sonra sizinle iletişime geçilir."),
                ("Üyelik", "Başvurular ne kadar sürede değerlendirilir?", "Başvurular, iletilen bilgi ve belgelerin eksiksiz olmasına bağlı olarak yönetim kurulunun değerlendirme takviminde sonuçlandırılır."),
                ("Etkinlikler", "Etkinliklere katılmak için üye olmak gerekir mi?", "Her etkinliğin katılım koşulu farklıdır. Herkese açık veya yalnızca üyelere özel olma durumu etkinlik ayrıntılarında belirtilir."),
                ("Kurumsal", "ELSADER hangi alanlarda faaliyet gösterir?", "ELSADER; sigorta araştırmaları, hasar incelemeleri, mesleki eğitim, etik standartlar, mevzuat takibi ve sektör paydaşlarıyla iş birliği alanlarında faaliyet yürütür."),
                ("İletişim", "Dernek yönetimine nasıl ulaşabilirim?", "İletişim sayfasındaki telefon ve e-posta bilgilerini kullanabilir veya iletişim formundan mesaj gönderebilirsiniz.")
            };

            for (var index = 0; index < defaults.Length; index++)
            {
                var item = defaults[index];
                context.CorporateContentSections.Add(new CorporateContentSection
                {
                    CorporatePageId = faqPage.Id,
                    SectionKey = $"faq-{Guid.NewGuid():N}",
                    Eyebrow = item.Item1,
                    Title = item.Item2,
                    ContentHtml = item.Item3,
                    DisplayOrder = index + 1,
                    IsActive = true,
                    UpdatedBy = "System"
                });
            }

            context.CorporateContentSections.Add(new CorporateContentSection
            {
                CorporatePageId = faqPage.Id,
                SectionKey = FaqSeedMarkerKey,
                Title = "SSS başlangıç kaydı",
                DisplayOrder = -1,
                IsActive = false,
                UpdatedBy = "System"
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task<CorporatePage> EnsurePageAsync(
        ApplicationDbContext context,
        string pageKey,
        string menuTitle,
        string heroTitle,
        int displayOrder,
        string? heroEyebrow = null,
        string? heroDescription = null)
    {
        var page = await context.CorporatePages.SingleOrDefaultAsync(item => item.PageKey == pageKey);
        if (page != null) return page;

        page = new CorporatePage
        {
            PageKey = pageKey,
            MenuTitle = menuTitle,
            HeroEyebrow = heroEyebrow,
            HeroTitle = heroTitle,
            HeroDescription = heroDescription,
            IsActive = false,
            DisplayOrder = displayOrder,
            UpdatedBy = "System"
        };
        context.CorporatePages.Add(page);
        await context.SaveChangesAsync();
        return page;
    }
}
