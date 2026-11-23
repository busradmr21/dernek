using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class PressContent
{
    public static IReadOnlyList<PressArticle> Items { get; } =
    [
        new PressArticle
        {
            Slug = "hasar-arastirmalarinda-ortak-standart",
            ImageUrl = "/images/multimedia/basin-aciklamasi.jpg",
            Category = "Basın Açıklaması",
            Title = "ELSADER’den Hasar Araştırmalarında Ortak Standart Çağrısı",
            Summary = "Saha incelemelerinde tarafsızlık, kanıta dayalı raporlama ve ortak mesleki standartların sektör güvenine katkısı ele alındı.",
            PublishedAt = new DateTime(2026, 8, 5),
            Paragraphs =
            [
                "Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu tarafından yayımlanan 2026/01 sayılı Kurum Sirküleri, hasar araştırma faaliyetlerinin görev sınırlarını açıklığa kavuşturdu. Düzenlemenin temel amacı, şüpheli hasarların ve sigorta suistimallerinin daha etkili biçimde incelenmesiyle piyasa disiplininin güçlendirilmesidir.",
                "Sirkülere göre hasar araştırmacılarının görev alanı sigorta suistimali ve şüpheli hasar incelemeleriyle sınırlıdır. Hasarın miktarını, nedenini ve niteliğini belirlemek; ön ekspertiz, kıymet tespiti ve hasar gözetimi yapmak ise sigorta eksperlerinin görev alanındadır. Bu ayrım, araştırma sürecinde yetki karmaşasını önlemek açısından önem taşır.",
                "ELSADER; görevlendirmenin kapsamının açıkça belirlenmesini, görüşme ve saha bulgularının doğrulanabilir biçimde kayıt altına alınmasını, raporlarda tespit ile değerlendirmelerin birbirinden ayrılmasını ve kişisel verilerin yalnızca gerekli olduğu ölçüde işlenmesini ortak mesleki standardın temel unsurları olarak görmektedir.",
                "Standart bir çalışma yaklaşımı; sigortalının haklarının korunmasına, araştırma raporlarının denetlenebilir olmasına ve sigorta şirketlerinin karar süreçlerinde daha tutarlı bilgi kullanmasına katkı sağlar. ELSADER bu amaçla örnek raporlama şablonları, etik ilkeler ve mesleki eğitim başlıkları üzerinde çalışmayı hedeflemektedir."
            ],
            Sources =
            [
                new PressSource
                {
                    Name = "SEDDK – Hasar Araştırmacıları Hakkında Kurum Sirküleri (2026/01)",
                    Url = "https://www.seddk.gov.tr/UploadContent/Documents/Hasar%20Ara%C5%9Ft%C4%B1rmac%C4%B1lar%C4%B1%20Kurum%20Sirk%C3%BCleri.pdf"
                }
            ]
        },
        new PressArticle
        {
            Slug = "sigorta-suistimalleri-saha-verisi",
            ImageUrl = "/images/multimedia/basin-roportaj.jpg",
            Category = "Röportaj",
            Title = "Sigorta Suistimalleriyle Mücadelede Saha Verisinin Önemi",
            Summary = "Şüpheli hasarların erken tespitinde uzman incelemesi, doğru kayıt ve kurumlar arası bilgi paylaşımının rolü değerlendirildi.",
            PublishedAt = new DateTime(2026, 8, 20),
            Paragraphs =
            [
                "Sigorta suistimalleriyle mücadelede yalnızca tek bir dosyaya bakmak çoğu zaman yeterli değildir. Saha görüşmeleri, olay yeri bulguları, belge kontrolleri ve geçmiş kayıtların aynı değerlendirme çerçevesinde ele alınması, tekrar eden örüntülerin ve tutarsızlıkların daha erken fark edilmesini sağlar.",
                "Sigorta Bilgi ve Gözetim Merkezi bünyesindeki Sigorta Suistimalleri Bilgi Paylaşım Sistemi (SİSBİS), yanlış sigorta uygulamaları ve suistimal şüphesi taşıyan kayıtların merkezi olarak değerlendirilmesine imkân verir. Ancak analizlerin güvenilirliği; sisteme aktarılan verinin doğruluğu, sınıflandırılması ve kaynağının izlenebilir olmasıyla doğrudan ilişkilidir.",
                "2026/27 sayılı Genelge ile oluşturulan Sigorta Suistimalleri ile Mücadele Komitesi ve Çalışma Heyeti; veri analizi, ortak risk göstergeleri, erken uyarı ölçütleri ve kurumlar arası iş birliği konularını öne çıkarmaktadır. Genelgede yapay zekâ, makine öğrenmesi ve ileri analitik uygulamalarına yönelik çalışmalar da mücadele araçları arasında sayılmaktadır.",
                "ELSADER’e göre saha uzmanının katkısı, yalnızca bilgi toplamak değil; bilginin ne zaman, nerede, kimden ve hangi yöntemle elde edildiğini açıkça gösterebilmektir. Standart kayıt yapısı, denetlenebilir raporlama ve gizlilik kuralları birlikte uygulandığında saha verisi sektör için daha anlamlı ve güvenilir hale gelir."
            ],
            Sources =
            [
                new PressSource
                {
                    Name = "SBM/SİSEB – Sigorta Suistimalleri ile Mücadele Komitesi Genelgesi",
                    Url = "https://siseb.sbm.org.tr/tr/sigorta-suistimalleri-ile-mucadele-genelgesi"
                },
                new PressSource
                {
                    Name = "SBM/SİSEB – Sigorta Suistimalleri Bilgi Paylaşım Sistemi",
                    Url = "https://siseb.sbm.org.tr/tr/sisbis"
                }
            ]
        },
        new PressArticle
        {
            Slug = "etik-ve-mesleki-gelisim-programi",
            ImageUrl = "/images/multimedia/basin-panel.jpg",
            Category = "Sektör Gündemi",
            Title = "Araştırmacılar İçin Etik ve Mesleki Gelişim Programı",
            Summary = "Sigorta araştırmacılarının güncel mevzuat, iletişim, etik ilkeler ve raporlama tekniklerindeki yetkinliğinin geliştirilmesi konuşuldu.",
            PublishedAt = new DateTime(2026, 9, 15),
            Paragraphs =
            [
                "Sigorta araştırmacılığı; mevzuat bilgisi, teknik gözlem, etkili iletişim ve tarafsız raporlama becerilerinin birlikte kullanılmasını gerektirir. Bu nedenle mesleki gelişimin yalnızca işe giriş dönemindeki eğitimlerle sınırlı kalmaması, düzenli yenileme programlarıyla desteklenmesi gerekir.",
                "SEGEM’in 2026 yılı eksper yenileme eğitimlerinde temel sigortacılık, mühendislik, yangın ve doğal afetler, kaza ve hırsızlık, kara araçları, nakliyat ile kredi ve emniyeti suistimal gibi farklı uzmanlık alanları yer almaktadır. Bu çeşitlilik, hasar süreçlerinin çok disiplinli yapısını göstermektedir.",
                "ELSADER’in önerdiği gelişim çerçevesinde; araştırmacının görev ve yetki sınırları, çıkar çatışmasından kaçınma, görüşme teknikleri, delil ve belge güvenliği, kişisel verilerin korunması, açık ve ölçülü rapor dili ile güncel suistimal yöntemleri temel eğitim başlıkları arasında yer almaktadır.",
                "Programların vaka çalışmaları ve örnek rapor incelemeleriyle desteklenmesi, teorik bilginin sahaya aktarılmasını kolaylaştırır. Eğitim sonunda yalnızca katılımın değil, öğrenme çıktılarının da ölçülmesi mesleki standardın sürdürülebilirliği açısından önemlidir."
            ],
            Sources =
            [
                new PressSource
                {
                    Name = "SEGEM – 2026 Yılı Eksper Yenileme Eğitimleri",
                    Url = "https://segem.org.tr/duyurular/2026-yili-eksper-yenileme-egitimleri"
                },
                new PressSource
                {
                    Name = "SEGEM – 2026 Yılı Yanlış Sigorta Uygulamaları Eğitimi",
                    Url = "https://segem.org.tr/duyurular/2026-yili-yanlis-sigorta-uygulamalari-egitimi"
                }
            ]
        },
        new PressArticle
        {
            Slug = "dijital-deliller-ve-veri-guvenligi",
            ImageUrl = "/images/multimedia/basin-soylesi.jpg",
            Category = "Uzman Görüşü",
            Title = "Dijital Deliller Sigorta İncelemelerinde Yeni Dönemi Başlatıyor",
            Summary = "Dijital kayıtların güvenli biçimde incelenmesi, veri bütünlüğü ve kişisel verilerin korunması konusunda uzman değerlendirmeleri paylaşıldı.",
            PublishedAt = new DateTime(2026, 10, 4),
            Paragraphs =
            [
                "Fotoğraf, video, konum, cihaz ve işlem kayıtları sigorta incelemelerinde giderek daha sık değerlendirilmektedir. Dijital bir kaydın anlamlı olabilmesi için yalnızca içeriğinin değil; kaynağının, elde edilme yönteminin, zaman bilgisinin ve inceleme boyunca bütünlüğünün de korunması gerekir.",
                "6698 sayılı Kişisel Verilerin Korunması Kanunu kapsamında veri sorumluları, kişisel verilerin hukuka aykırı işlenmesini ve erişilmesini önlemek, ayrıca verilerin güvenli biçimde saklanmasını sağlamakla yükümlüdür. Bu yükümlülük, sigorta dosyalarındaki kimlik, iletişim, sağlık, konum ve finansal kayıtlar bakımından özel önem taşır.",
                "KVKK’nın Kişisel Veri Güvenliği Rehberi; erişim yetkilerinin sınırlandırılması, log kayıtları, veri maskeleme, yedekleme, güvenlik duvarları, güncel zararlı yazılım koruması, güvenli silme ve anahtar yönetimi gibi teknik tedbirleri öne çıkarmaktadır. Risk analizi, kurumsal politikalar, gizlilik taahhütleri ve düzenli denetimler ise idari tedbirlerin parçasıdır.",
                "ELSADER, dijital incelemelerde amaçla sınırlılık ve veri minimizasyonu ilkelerinin uygulanmasını; dosyaya erişen kişilerin kayıt altına alınmasını ve orijinal kaydın değiştirilmeden korunmasını temel yaklaşım olarak benimsemektedir. Böylece hem incelemenin güvenilirliği hem de ilgili kişilerin mahremiyeti korunabilir."
            ],
            Sources =
            [
                new PressSource
                {
                    Name = "KVKK – Kullanıcı Güvenliğine İlişkin Teknik ve İdari Tedbirler",
                    Url = "https://www.kvkk.gov.tr/Icerik/7177/Kullanici-Guvenligine-Iliskin-Veri-Sorumlulari-Tarafindan-Alinmasi-Tavsiye-Edilen-Teknik-ve-Idari-Tedbirlere-Iliskin-Kamuoyu-Duyurusu"
                },
                new PressSource
                {
                    Name = "KVKK – Kişisel Veri Güvenliği Rehberi",
                    Url = "https://www.kvkk.gov.tr/SharedFolderServer/CMSFiles/7512d0d4-f345-41cb-bc5b-8d5cf125e3a1.pdf"
                }
            ]
        }
    ];
}
