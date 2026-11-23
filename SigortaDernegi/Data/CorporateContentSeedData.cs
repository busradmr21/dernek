using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class CorporateContentSeedData
{
    private const string SeedMarkerKey = "__corporate_content_seed_complete";
    private const string AboutRedesignMarkerKey = "__about_redesign_v1";
    private const string AboutLayoutRevisionMarkerKey = "__about_layout_revision_v2";
    private const string AboutPurposeRevisionMarkerKey = "__about_purpose_revision_v3";
    private const string BylawsRedesignMarkerKey = "__bylaws_redesign_v1";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var pageDefinitions = new[]
        {
            Page("about", "Hakkımızda", "Kurumsal", "Hakkımızda", "Sigorta araştırmacılarını ve uzmanlarını ortak değerler etrafında buluşturan, mesleki gelişimi ve etik standartları destekleyen güçlü bir dayanışma platformuyuz.", 1),
            Page("bylaws", "Tüzük ve Etik İlkeler", "Kurumsal İlkeler", "Dernek Tüzüğü ve İlkeler", "ELSADER’in kurumsal çalışma esasları, mesleki etik değerleri ve üyelerinin faaliyetlerinde gözetmesi gereken temel sorumluluklar.", 2),
            Page("legislation", "Mevzuat", "Resmî Kaynak Rehberi", "Mevzuat", "Dernek faaliyetleri, sigortacılık sektörü ve kişisel verilerin korunmasıyla ilgili temel düzenlemelere resmî kurumların kaynaklarından ulaşabilirsiniz.", 3),
            Page("board", "Yönetim Kurulu", "Yönetim ve Temsil", "Yönetim Kurulu", "ELSADER’in amaçları doğrultusunda çalışmalarını yürüten, kurumsal kararlarını alan ve derneği temsil eden yönetim kadrosu.", 4),
            Page("committees", "Komisyonlar", "Uzmanlık ve İş Birliği", "Komisyonlar", "ELSADER bünyesinde farklı uzmanlık alanlarında çalışmalar yürüten komisyonlar; bilgi paylaşımına, mesleki gelişime ve sektörel standartların güçlenmesine katkı sağlar.", 5)
        };

        var existingKeys = await context.CorporatePages.Select(page => page.PageKey).ToListAsync();
        var missingPages = pageDefinitions
            .Where(page => !existingKeys.Contains(page.PageKey, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missingPages.Count > 0)
        {
            context.CorporatePages.AddRange(missingPages);
            await context.SaveChangesAsync();
        }

        var pages = await context.CorporatePages.ToDictionaryAsync(page => page.PageKey);

        if (string.Equals(
                pages["about"].HeroTitle,
                "ELSADER Hakkında",
                StringComparison.Ordinal))
        {
            pages["about"].HeroTitle = "Hakkımızda";
            await context.SaveChangesAsync();
        }

        await EnsureAboutRedesignAsync(context, pages["about"]);
        await EnsureAboutLayoutRevisionAsync(context, pages["about"]);
        await EnsureAboutPurposeRevisionAsync(context, pages["about"]);
        await EnsureBylawsRedesignAsync(context, pages["bylaws"]);

        if (await context.CorporateContentSections.AnyAsync(
                section => section.SectionKey == SeedMarkerKey))
        {
            return;
        }

        var sectionDefinitions = new[]
        {
            Section(pages["about"].Id, "purpose", "Neden ELSADER?", "Kuruluş Amacımız", "<p>ELSADER; sigorta araştırmacıları ve uzmanları arasında mesleki dayanışmayı geliştirmek, bilgi paylaşımını güçlendirmek ve sektörde ortak çalışma standartlarının oluşmasına katkı sağlamak amacıyla kurulmuştur.</p><p>Derneğimiz; sigorta suistimalleriyle mücadele, saha ve hasar inceleme çalışmalarının niteliğinin yükseltilmesi, etik ilkelerin yaygınlaştırılması ve sektör paydaşları arasındaki iş birliğinin artırılması için çalışmalar yürütür.</p>", 1),
            Section(pages["about"].Id, "mission", "Misyonumuz", "Mesleki gelişimi ve etik çalışma kültürünü güçlendirmek", "<p>Sigorta araştırmacıları ve uzmanlarının mesleki gelişimlerini desteklemek, ortak çalışma standartları oluşturmak, etik ilkeleri yaygınlaştırmak ve sektör içindeki iş birliğini güçlendirmek.</p>", 2),
            Section(pages["about"].Id, "vision", "Vizyonumuz", "Güvenilir ve öncü bir meslek kuruluşu olmak", "<p>Bilgi, deneyim ve dayanışma gücüyle sigorta araştırmaları alanında güvenilir, yenilikçi ve sektöre yön veren bir meslek kuruluşu olmak.</p>", 3),
            Section(pages["about"].Id, "values", "Ortak Değerlerimiz", "Çalışmalarımıza yön veren ilkeler", "<ul><li>Tarafsızlık ve güvenilirlik</li><li>Mesleki etik ve sorumluluk</li><li>Bilgi paylaşımı ve dayanışma</li><li>Sürekli gelişim ve yenilikçilik</li></ul>", 4),
            Section(pages["bylaws"].Id, "bylaws", "Kurumsal Çerçeve", "Dernek Tüzüğü", "<p>Derneğin amacı, çalışma alanları, üyelik koşulları ve yönetim esasları dernek tüzüğünde belirlenir. ELSADER tüm faaliyetlerini yürürlükteki mevzuata ve tüzüğüne uygun şekilde yürütür.</p>", 1),
            Section(pages["bylaws"].Id, "ethics", "Mesleki Sorumluluk", "Etik İlkelerimiz", "<ul><li>Tarafsız ve bağımsız hareket etmek</li><li>Kişisel verileri ve mesleki sırları korumak</li><li>Çıkar çatışmalarından kaçınmak</li><li>Raporlama süreçlerinde doğruluk ve şeffaflığı esas almak</li><li>Mesleki yeterliliği sürekli geliştirmek</li></ul>", 2),
            Section(pages["bylaws"].Id, "privacy", "Bilgi Güvenliği", "Gizlilik ve Kişisel Veriler", "<p>Üyelerimiz görevleri sırasında edindikleri kişisel, ticari ve mesleki bilgileri yalnızca yetkili amaçlarla kullanır; bu bilgilerin gizliliğini ve güvenliğini korur.</p>", 3),
            Section(pages["legislation"].Id, "associations", "Resmî Kaynaklar", "Dernekler Mevzuatı", "<p><a href=\"https://www.siviltoplum.gov.tr/kanunlar\" target=\"_blank\" rel=\"noopener\">Derneklerle ilgili kanunlar</a></p><p><a href=\"https://www.siviltoplum.gov.tr/yonetmelik\" target=\"_blank\" rel=\"noopener\">Dernekler yönetmeliği</a></p>", 1),
            Section(pages["legislation"].Id, "insurance", "Sektörel Düzenlemeler", "Sigortacılık Mevzuatı", "<p><a href=\"https://www.seddk.gov.tr/tr/mevzuat/sigortacilik/kanunlar-ve-khk\" target=\"_blank\" rel=\"noopener\">Sigortacılık kanunları ve KHK</a></p><p><a href=\"https://www.seddk.gov.tr/tr/mevzuat/sigortacilik/genelge-teblig-ve-sektor-duyurulari\" target=\"_blank\" rel=\"noopener\">Genelge, tebliğ ve sektör duyuruları</a></p>", 2),
            Section(pages["legislation"].Id, "privacy-law", "Veri Güvenliği", "Kişisel Verilerin Korunması", "<p><a href=\"https://www.kvkk.gov.tr/Icerik/2050/Kisisel-Veriler\" target=\"_blank\" rel=\"noopener\">Kişisel veriler hakkında bilgi</a></p><p><a href=\"https://www.kvkk.gov.tr/Icerik/6554/Kisisel-Verilerin-Korunmasi-Kanunu-Uyum-Surecine-Iliskin-Duyuru\" target=\"_blank\" rel=\"noopener\">KVKK uyum süreci</a></p>", 3),
            Section(pages["board"].Id, "chair", "Yönetim Kurulu Başkanı", "Ad Soyad", "<p>Üye bilgisi daha sonra eklenecektir.</p>", 1),
            Section(pages["board"].Id, "vice-chair", "Yönetim Kurulu Başkan Yardımcısı", "Ad Soyad", "<p>Üye bilgisi daha sonra eklenecektir.</p>", 2),
            Section(pages["board"].Id, "secretary", "Yönetim Kurulu Üyesi", "Ad Soyad", "<p>Üye bilgisi daha sonra eklenecektir.</p>", 3),
            Section(pages["board"].Id, "auditor-1", "Denetim Kurulu", "Ad Soyad", "<p>Denetim kurulu üyesi.</p>", 4),
            Section(pages["board"].Id, "auditor-2", "Denetim Kurulu", "Ad Soyad", "<p>Denetim kurulu üyesi.</p>", 5),
            Section(pages["committees"].Id, "research", "Araştırma", "Sigorta Araştırmaları Komisyonu", "<p>Sigorta araştırmaları, hasar incelemeleri ve sektörel gelişmeler hakkında çalışma ve raporlar hazırlar.</p>", 1),
            Section(pages["committees"].Id, "education", "Mesleki Gelişim", "Eğitim Komisyonu", "<p>Üyelerin mesleki gelişimini destekleyen eğitim, seminer ve bilgilendirme programlarını planlar.</p>", 2),
            Section(pages["committees"].Id, "ethics", "Standartlar", "Etik ve Disiplin Komisyonu", "<p>Mesleki etik ilkelerin geliştirilmesi ve uygulanmasına yönelik çalışmalar yürütür.</p>", 3),
            Section(pages["committees"].Id, "digital", "Gelecek", "Dijital Dönüşüm Komisyonu", "<p>Teknoloji, veri güvenliği ve dijital çalışma yöntemleri konusunda sektörel gelişmeleri takip eder.</p>", 4)
        };

        var existingSectionKeys = await context.CorporateContentSections
            .Select(section => new { section.CorporatePageId, section.SectionKey })
            .ToListAsync();
        var missingSections = sectionDefinitions
            .Where(section => !existingSectionKeys.Any(existing => existing.CorporatePageId == section.CorporatePageId && existing.SectionKey == section.SectionKey))
            .ToList();

        if (missingSections.Count > 0)
        {
            context.CorporateContentSections.AddRange(missingSections);
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = pages["about"].Id,
            SectionKey = SeedMarkerKey,
            Title = "Kurumsal içerik başlangıç kaydı",
            DisplayOrder = -1,
            IsActive = false,
            UpdatedBy = "System"
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAboutRedesignAsync(
        ApplicationDbContext context,
        CorporatePage aboutPage)
    {
        if (await context.CorporateContentSections.AnyAsync(section =>
                section.CorporatePageId == aboutPage.Id &&
                section.SectionKey == AboutRedesignMarkerKey))
        {
            return;
        }

        aboutPage.HeroTitle = "Hakkımızda";
        aboutPage.HeroDescription = "Uzmanlığı, güveni ve mesleki dayanışmayı ortak bir çatı altında buluşturuyoruz.";
        aboutPage.UpdatedAt = DateTime.UtcNow;
        aboutPage.UpdatedBy = "System";

        var desiredSections = new[]
        {
            Section(
                aboutPage.Id,
                "identity",
                "ELSADER Kimdir?",
                "Uzmanlığı, güveni ve mesleki dayanışmayı ortak bir çatı altında buluşturuyoruz.",
                """
                <p>ELSADER; sigorta araştırmaları, hasar inceleme süreçleri, suistimal risklerinin değerlendirilmesi ve mesleki uzmanlık alanlarında çalışan kişi ve kuruluşları ortak değerler etrafında buluşturan bir sivil toplum kuruluşudur.</p>
                <p>Derneğimiz; sigorta sektöründe yürütülen araştırma ve inceleme çalışmalarının nesnel verilere, hukuka, mesleki etik ilkelere ve bilgi güvenliğine uygun biçimde gerçekleştirilmesini destekler. Üyeler arasında bilgi ve deneyim paylaşımını güçlendirirken sigorta şirketleri, uzmanlar, akademik çevreler, kamu kurumları ve diğer sektör paydaşlarıyla yapıcı iş birliklerinin geliştirilmesine önem verir.</p>
                <p>ELSADER herhangi bir uyuşmazlığın tarafı veya karar mercii olmayı değil; uzmanlık kapasitesini artıran, ortak çalışma kültürünü geliştiren ve güvenilir uygulamaların yaygınlaşmasına katkı sağlayan bağımsız bir mesleki dayanışma platformu olmayı esas alır.</p>
                """,
                1),
            Section(
                aboutPage.Id,
                "purpose",
                "Neden ELSADER?",
                "Kuruluş Amacımız",
                """
                <p>ELSADER’in temel kuruluş amacı; sigorta araştırmaları ve hasar inceleme alanlarında görev yapan uzmanların mesleki birikimini ortak bir çatı altında toplamak, uygulamada güvenilir ve ölçülebilir standartların oluşmasına katkı sağlamak ve sektör içindeki iş birliğini güçlendirmektir.</p>
                <p>Dernek; hasar olaylarının teknik, hukuki ve etik boyutlarıyla değerlendirilmesini desteklemeyi, gerçeğe aykırı beyan ve belgelerden kaynaklanabilecek suistimal risklerine karşı mesleki farkındalığı artırmayı ve araştırma süreçlerinde tarafsızlık, doğruluk, gizlilik ile delile dayalı çalışma ilkelerini yaygınlaştırmayı hedefler.</p>
                <p>Bu amaç doğrultusunda eğitim, seminer, çalışma grubu, araştırma, yayın ve sektörel toplantılar düzenlenmesi; üyelerin güncel mevzuat ve teknolojik gelişmeler konusunda desteklenmesi; ortak sorunlara çözüm üretecek mesleki görüşlerin oluşturulması ve ilgili kurumlarla sürdürülebilir iletişim kanallarının kurulması öncelikli çalışma alanlarımız arasındadır.</p>
                """,
                2),
            Section(
                aboutPage.Id,
                "mission",
                "Misyonumuz",
                "Bilgiye ve etik ilkelere dayalı mesleki gelişimi güçlendirmek",
                """
                <p>Sigorta araştırmacıları ve uzmanlarının mesleki bilgi, teknik yeterlilik ve kurumsal sorumluluklarını geliştirmek; inceleme ve raporlama süreçlerinde tarafsızlık, doğruluk, gizlilik ve hesap verebilirlik ilkelerinin benimsenmesini sağlamak temel misyonumuzdur.</p>
                <p>Eğitimler, uzmanlık çalışmaları, yayınlar ve paydaş iş birlikleri yoluyla üyelerimizin güncel mevzuata, yeni risk türlerine ve teknolojik gelişmelere uyumunu destekler; sigorta suistimalleriyle mücadelede bilinçli, koordineli ve hak temelli bir yaklaşımın yaygınlaşmasına katkı sunarız.</p>
                """,
                3),
            Section(
                aboutPage.Id,
                "vision",
                "Vizyonumuz",
                "Güvenilir uygulamalara yön veren öncü bir meslek kuruluşu olmak",
                """
                <p>Sigorta araştırmaları ve hasar inceleme alanında ulusal ölçekte güven duyulan, görüşleri ve çalışmaları referans alınan, yenilikçi ve sürdürülebilir bir meslek kuruluşu olmayı amaçlıyoruz.</p>
                <p>Mesleki standartların sürekli geliştiği, bilimsel yöntemlerin ve teknolojinin sorumlu biçimde kullanıldığı, paydaşlar arasında şeffaf iş birliğinin güçlendiği bir sektör yapısına öncülük etmek; dürüst sigortalının haklarının ve sigorta sistemine duyulan güvenin korunmasına uzun vadeli katkı sağlamak vizyonumuzun temelini oluşturur.</p>
                """,
                4),
            Section(
                aboutPage.Id,
                "values",
                "Ortak Değerlerimiz",
                "Her çalışmamıza yön veren temel ilkeler",
                """
                <ul>
                    <li><strong>Tarafsızlık ve Güvenilirlik</strong><span>İnceleme ve değerlendirmelerde nesnel verileri esas alır, tüm taraflara eşit mesafede dururuz.</span></li>
                    <li><strong>Mesleki Etik ve Sorumluluk</strong><span>Hukuka, meslek etiğine, kişisel haklara ve bilgi güvenliği yükümlülüklerine uygun hareket ederiz.</span></li>
                    <li><strong>Bilgi Paylaşımı ve Dayanışma</strong><span>Uzmanlık bilgisini ortak faydaya dönüştüren açık, yapıcı ve sürdürülebilir iş birliklerini destekleriz.</span></li>
                    <li><strong>Sürekli Gelişim ve Yenilikçilik</strong><span>Mevzuatı, teknolojiyi ve yeni riskleri izler; mesleki kapasitenin sürekli gelişmesini önemseriz.</span></li>
                </ul>
                """,
                5)
        };

        var currentSections = await context.CorporateContentSections
            .Where(section => section.CorporatePageId == aboutPage.Id)
            .ToListAsync();

        foreach (var desiredSection in desiredSections)
        {
            var currentSection = currentSections.SingleOrDefault(section =>
                section.SectionKey == desiredSection.SectionKey);

            if (currentSection == null)
            {
                context.CorporateContentSections.Add(desiredSection);
                continue;
            }

            currentSection.Eyebrow = desiredSection.Eyebrow;
            currentSection.Title = desiredSection.Title;
            currentSection.ContentHtml = desiredSection.ContentHtml;
            currentSection.ImageUrl = desiredSection.ImageUrl;
            currentSection.DisplayOrder = desiredSection.DisplayOrder;
            currentSection.IsActive = true;
            currentSection.UpdatedAt = DateTime.UtcNow;
            currentSection.UpdatedBy = "System";
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = aboutPage.Id,
            SectionKey = AboutRedesignMarkerKey,
            Title = "Hakkımızda tasarım ve içerik güncellemesi",
            DisplayOrder = -2,
            IsActive = false,
            UpdatedBy = "System"
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAboutLayoutRevisionAsync(
        ApplicationDbContext context,
        CorporatePage aboutPage)
    {
        if (await context.CorporateContentSections.AnyAsync(section =>
                section.CorporatePageId == aboutPage.Id &&
                section.SectionKey == AboutLayoutRevisionMarkerKey))
        {
            return;
        }

        aboutPage.HeroDescription = "Uzmanlığı, güveni ve mesleki dayanışmayı ortak bir çatı altında buluşturuyoruz.";
        aboutPage.UpdatedAt = DateTime.UtcNow;
        aboutPage.UpdatedBy = "System";

        var identitySection = await context.CorporateContentSections.SingleOrDefaultAsync(section =>
            section.CorporatePageId == aboutPage.Id &&
            section.SectionKey == "identity");

        if (identitySection is not null)
        {
            identitySection.ImageUrl = null;
            identitySection.UpdatedAt = DateTime.UtcNow;
            identitySection.UpdatedBy = "System";
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = aboutPage.Id,
            SectionKey = AboutLayoutRevisionMarkerKey,
            Title = "Hakkımızda metin yerleşimi güncellemesi",
            DisplayOrder = -3,
            IsActive = false,
            UpdatedBy = "System"
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAboutPurposeRevisionAsync(
        ApplicationDbContext context,
        CorporatePage aboutPage)
    {
        if (await context.CorporateContentSections.AnyAsync(section =>
                section.CorporatePageId == aboutPage.Id &&
                section.SectionKey == AboutPurposeRevisionMarkerKey))
        {
            return;
        }

        var purposeSection = await context.CorporateContentSections.SingleOrDefaultAsync(section =>
            section.CorporatePageId == aboutPage.Id &&
            section.SectionKey == "purpose");

        if (purposeSection is not null)
        {
            purposeSection.Eyebrow = null;
            purposeSection.UpdatedAt = DateTime.UtcNow;
            purposeSection.UpdatedBy = "System";
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = aboutPage.Id,
            SectionKey = AboutPurposeRevisionMarkerKey,
            Title = "Kuruluş amacı üst etiket güncellemesi",
            DisplayOrder = -4,
            IsActive = false,
            UpdatedBy = "System"
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureBylawsRedesignAsync(
        ApplicationDbContext context,
        CorporatePage bylawsPage)
    {
        if (await context.CorporateContentSections.AnyAsync(section =>
                section.CorporatePageId == bylawsPage.Id &&
                section.SectionKey == BylawsRedesignMarkerKey))
        {
            return;
        }

        bylawsPage.MenuTitle = "Tüzük ve Etik İlkeler";
        bylawsPage.HeroEyebrow = "Kurumsal İlkeler";
        bylawsPage.HeroTitle = "Dernek Tüzüğü ve Etik İlkeler";
        bylawsPage.HeroDescription = "ELSADER’in kuruluş amacı, çalışma esasları, organları, üyelik düzeni ve mesleki etik yükümlülüklerini belirleyen kurumsal çerçeve.";
        bylawsPage.IsActive = true;
        bylawsPage.UpdatedAt = DateTime.UtcNow;
        bylawsPage.UpdatedBy = "System";

        var desiredSections = new[]
        {
            Section(
                bylawsPage.Id,
                "intro",
                "Kurumsal Güvence",
                "Şeffaf, hesap verebilir ve etik bir çalışma düzeni",
                """
                <p>ELSADER tüzüğü; sigorta araştırmaları, hasar inceleme, risk değerlendirme ve suistimalle mücadele alanlarında çalışan üyelerin ortak çalışma düzenini belirler.</p>
                <p>Derneğin organları, karar alma süreçleri, mali sorumlulukları ve üyelik hükümleri açık kurallara bağlanır. Etik ilkeler ise her araştırma ve incelemede bağımsızlığı, nesnelliği, gizliliği ve delile dayalı raporlamayı güvence altına alır.</p>
                """,
                1),
            Section(
                bylawsPage.Id,
                "bylaws-text",
                "ELSADER Dernek Tüzüğü",
                "Temel Hükümler",
                """
                <p>Aşağıdaki metin, derneğin amacı ve çalışma alanlarıyla birlikte üyelik, yönetim, denetim ve mali düzenine ilişkin temel esasları açıklar.</p>

                <h3>1. Bölüm</h3>
                <h4>Derneğin adı ve merkezi</h4>
                <p>Derneğin adı “Elazığ Sigorta Acenteleri Derneği”, kısa adı “ELSADER”dir. Derneğin merkezi Türkiye’dedir. Merkez adresi ve yapılacak adres değişiklikleri ilgili mevzuatta öngörülen usule göre yetkili mercilere bildirilir.</p>

                <h4>Derneğin amacı</h4>
                <p>ELSADER; sigorta araştırmaları, hasar inceleme, risk değerlendirme, adli analiz ve suistimalle mücadele alanlarında mesleki gelişimi desteklemek; bilimsel doğruluğu, tarafsızlığı ve etik çalışma kültürünü yaygınlaştırmak amacıyla kurulmuştur.</p>
                <p>Dernek, üyeleri ile sektör paydaşları arasında bilgi ve deneyim paylaşımını güçlendirir; güvenilir uygulama standartlarının oluşmasına ve sigortalılık bilincinin gelişmesine katkı sağlar.</p>

                <h4>Çalışma konuları ve faaliyet biçimleri</h4>
                <ul>
                    <li>Mesleki eğitim, seminer, çalıştay, konferans ve webinar düzenlemek,</li>
                    <li>Araştırma, rapor, rehber, standart ve bilimsel yayın hazırlamak,</li>
                    <li>Kamu kurumları, üniversiteler ve sektör kuruluşlarıyla ortak çalışmalar yürütmek,</li>
                    <li>Meslek etiği, kişisel verilerin korunması ve bilgi güvenliği alanlarında farkındalık oluşturmak.</li>
                </ul>

                <h3>2. Bölüm</h3>
                <h4>Üyelik koşulları ve başvuru</h4>
                <p>Derneğin amaç ve ilkelerini benimseyen, ilgili mesleki alanlarda faaliyet gösteren ve mevzuatta üyelik için aranan şartları taşıyan gerçek veya tüzel kişiler üyelik başvurusunda bulunabilir. Başvurular yönetim kurulunca değerlendirilir ve sonuç başvuru sahibine yazılı olarak bildirilir.</p>

                <h4>Üyelerin hak ve yükümlülükleri</h4>
                <p>Her üye dernek faaliyetlerine ve genel kurula katılma, oy kullanma ve dernek çalışmaları hakkında bilgi edinme hakkına sahiptir. Üyeler tüzüğe, etik ilkelere ve yetkili organ kararlarına uymak; aidat yükümlülüklerini yerine getirmek ve derneğin kurumsal itibarını korumakla yükümlüdür.</p>

                <h4>Üyeliğin sona ermesi</h4>
                <p>Üyelik; yazılı ayrılma bildirimi, üyelik şartlarının kaybedilmesi veya tüzükte gösterilen haklı sebeplerin gerçekleşmesi hâlinde sona erebilir. Çıkarma işlemlerinde üyeye açıklama ve savunma hakkı tanınır; karar gerekçeli olarak bildirilir.</p>

                <h3>3. Bölüm</h3>
                <h4>Derneğin organları</h4>
                <p>Derneğin zorunlu organları genel kurul, yönetim kurulu ve denetim kuruludur. Derneğin amaçları doğrultusunda bilim, etik, eğitim ve araştırma komisyonları kurulabilir; bu yapıların görev ve yetkileri yönetim kurulu kararıyla belirlenir.</p>

                <h4>Genel kurul</h4>
                <p>Genel kurul, derneğin en yetkili karar organıdır. Tüzükte belirtilen zamanlarda olağan; yönetim veya denetim kurulunun gerekli gördüğü hâllerde ya da mevzuatta belirtilen üye talebi üzerine olağanüstü toplanır. Bütçe, faaliyet raporu, organ seçimleri ve tüzük değişiklikleri genel kurulda karara bağlanır.</p>

                <h4>Yönetim kurulu</h4>
                <p>Yönetim kurulu derneği temsil eder; faaliyet planını, bütçeyi, üyelik süreçlerini ve kurumsal iş birliklerini yürütür. Kararlar toplantı ve karar yeter sayıları gözetilerek alınır, karar defterine işlenir ve düzenli biçimde raporlanır.</p>

                <h4>Denetim kurulu</h4>
                <p>Denetim kurulu, dernek faaliyetlerinin tüzüğe ve mevzuata uygunluğunu; hesap, kayıt ve belgelerin usulüne göre tutulup tutulmadığını belirlenen dönemlerde inceler. Denetim sonuçlarını rapor hâlinde yönetim kuruluna ve toplandığında genel kurula sunar.</p>

                <h3>4. Bölüm</h3>
                <h4>Gelirler, giderler ve mali şeffaflık</h4>
                <p>Derneğin gelirleri; üye aidatları, bağış ve yardımlar, etkinlik ve yayın gelirleri ile mevzuata uygun diğer kaynaklardan oluşur. Gelir ve gider işlemleri belgelendirilir; bütçe, harcama yetkisi, kayıt düzeni ve iç denetim şeffaflık ilkesi doğrultusunda yürütülür.</p>

                <h4>Tüzük değişikliği, fesih ve tasfiye</h4>
                <p>Tüzük değişiklikleri genel kurul gündeminde görüşülür ve ilgili mevzuatta öngörülen karar yeter sayısıyla kabul edilir. Derneğin feshi hâlinde tasfiye işlemleri genel kurulca belirlenecek tasfiye kurulu tarafından yürütülür; malvarlığının devri genel kurul kararı ve yürürlükteki mevzuata göre tamamlanır.</p>
                """,
                2),
            Section(
                bylawsPage.Id,
                "ethics-text",
                "Mesleki Davranış Esasları",
                "Etik İlkelerimiz",
                """
                <p>ELSADER üyeleri, bütün mesleki çalışmalarda kamu yararını, adaleti ve güvenilir bilgi üretimini esas alır.</p>

                <h3>Tarafsızlık ve bağımsızlık</h3>
                <p>İnceleme ve değerlendirmeler kişisel, ticari veya kurumsal baskılardan bağımsız yürütülür; tüm taraflara eşit mesafede durulur.</p>

                <h3>Doğruluk ve delile dayalı raporlama</h3>
                <p>Bulgular doğrulanabilir bilgi, belge ve teknik incelemeye dayanır. Varsayımlar, kanaatler ve kesin tespitler raporda açık biçimde birbirinden ayrılır.</p>

                <h3>Gizlilik ve kişisel verilerin korunması</h3>
                <p>Kişisel veriler, ticari sırlar ve dosya içerikleri yalnızca yetki ve amaç kapsamında işlenir; yetkisiz kişilerle paylaşılmaz ve güvenli biçimde saklanır.</p>

                <h3>Çıkar çatışmasından kaçınma</h3>
                <p>Tarafsızlığı etkileyebilecek her türlü ilişki veya menfaat gecikmeden açıklanır; gerekli hâllerde görev kabul edilmez ya da dosyadan çekilinir.</p>

                <h3>Mesleki yeterlilik ve sürekli gelişim</h3>
                <p>Üyeler yalnızca yetkin oldukları alanlarda görev üstlenir; mevzuat, teknoloji, inceleme yöntemleri ve mesleki standartlardaki gelişmeleri düzenli olarak takip eder.</p>

                <h3>Saygı ve kurumsal sorumluluk</h3>
                <p>Ayrımcılığa, yanıltıcı beyana, haksız rekabete ve meslek itibarını zedeleyen davranışlara izin verilmez. Etik ihlal bildirimleri gizlilik ve savunma hakkı gözetilerek incelenir.</p>
                """,
                3),
            Section(
                bylawsPage.Id,
                "document",
                "Belge",
                "ELSADER Dernek Tüzüğü ve Etik İlkeler",
                "<p>Tam metni PDF biçiminde görüntüleyebilir, cihazınıza indirebilir veya yazdırabilirsiniz.</p>",
                4,
                "/documents/hisad-dernek-tuzugu-ve-etik-ilkeler.pdf")
        };

        var currentSections = await context.CorporateContentSections
            .Where(section => section.CorporatePageId == bylawsPage.Id)
            .ToListAsync();

        foreach (var desiredSection in desiredSections)
        {
            var currentSection = currentSections.SingleOrDefault(section =>
                section.SectionKey == desiredSection.SectionKey);

            if (currentSection is null)
            {
                context.CorporateContentSections.Add(desiredSection);
                continue;
            }

            currentSection.Eyebrow = desiredSection.Eyebrow;
            currentSection.Title = desiredSection.Title;
            currentSection.ContentHtml = desiredSection.ContentHtml;
            currentSection.ImageUrl = desiredSection.ImageUrl;
            currentSection.DisplayOrder = desiredSection.DisplayOrder;
            currentSection.IsActive = true;
            currentSection.UpdatedAt = DateTime.UtcNow;
            currentSection.UpdatedBy = "System";
        }

        var desiredKeys = desiredSections
            .Select(section => section.SectionKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var obsoleteSection in currentSections.Where(section =>
                     !section.SectionKey.StartsWith("__", StringComparison.Ordinal) &&
                     !desiredKeys.Contains(section.SectionKey)))
        {
            obsoleteSection.IsActive = false;
            obsoleteSection.UpdatedAt = DateTime.UtcNow;
            obsoleteSection.UpdatedBy = "System";
        }

        context.CorporateContentSections.Add(new CorporateContentSection
        {
            CorporatePageId = bylawsPage.Id,
            SectionKey = BylawsRedesignMarkerKey,
            Title = "Tüzük ve etik ilkeler sayfa güncellemesi",
            DisplayOrder = -5,
            IsActive = false,
            UpdatedBy = "System"
        });

        await context.SaveChangesAsync();
    }

    private static CorporatePage Page(string key, string menuTitle, string eyebrow, string title, string description, int order) =>
        new()
        {
            PageKey = key,
            MenuTitle = menuTitle,
            HeroEyebrow = eyebrow,
            HeroTitle = title,
            HeroDescription = description,
            DisplayOrder = order,
            IsActive = true
        };

    private static CorporateContentSection Section(
        int pageId,
        string key,
        string eyebrow,
        string title,
        string content,
        int order,
        string? imageUrl = null) =>
        new()
        {
            CorporatePageId = pageId,
            SectionKey = key,
            Eyebrow = eyebrow,
            Title = title,
            ContentHtml = content,
            ImageUrl = imageUrl,
            DisplayOrder = order,
            IsActive = true
        };
}
