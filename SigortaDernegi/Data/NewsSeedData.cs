using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data
{
    public static class NewsSeedData
    {
        private const string SeedMarkerTitle = "__news_seed_complete";

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.News.AnyAsync(item => item.Title == SeedMarkerTitle))
            {
                return;
            }

            var newsItems = new List<News>
            {
                CreateNews(
                    "Sigorta Hasar Bilgileri e-Devlet Üzerinden Erişime Açıldı",
                    "Eksper raporları, kaza tespit tutanakları, kusur oranları ve poliçe bilgileri e-Devlet üzerinden görüntülenebilecek.",
                    "Sigortacılık ve Özel Emeklilik Düzenleme ve Denetleme Kurumu, e-Devlet üzerindeki Özel Sigortalarım hizmetinin ikinci fazının tamamlandığını duyurdu. Yeni düzenlemeyle vatandaşlar hasar dosyalarına ait eksper raporlarına, kaza tespit tutanaklarına ve kusur oranlarına erişebilecek.\n\nHizmet kapsamında tüm branşlardaki güncel ve geçmiş poliçelerin ayrıntıları ile poliçe yenileme uyarıları da görüntülenebilecek. Bu gelişme, hasar süreçlerinin daha şeffaf izlenmesine ve sigortalıların kendi dosyalarına daha hızlı ulaşmasına katkı sağlayacak.\n\nKaynak: https://seddk.gov.tr/UploadContent/Documents/ozel_sigortalarim_seddk_basin_duyurusu_.pdf",
                    "/images/news/news-edevlet.jpg",
                    new DateTime(2026, 10, 5, 10, 0, 0)),

                CreateNews(
                    "Sigorta Hasar Çetelerine Yönelik Çok Katmanlı Mücadele Sürüyor",
                    "SEDDK; standart ekspertiz, akıllı eksper ataması, ortak ihbar hattı ve denetim çalışmalarıyla hasar istismarına karşı yeni adımları açıkladı.",
                    "SEDDK, trafik kazaları sonrasındaki tazmin süreçlerini istismar eden yapılara karşı mevzuat, uygulama ve teknoloji alanlarında yürütülen çalışmaları kamuoyuyla paylaştı. Açıklamada ekspertiz hesaplamalarının gerçek zarar ilkesine göre standartlaştırılması ve kazaya uygun eksperin otomasyonla görevlendirilmesi öne çıktı.\n\nHasar takibinin yeniden düzenlenmesi, değer kaybı başvurularındaki ilave şartların kaldırılması ve Alo 193 Ortak Hasar İhbar Hattının kurulması da mücadele planının parçaları arasında gösterildi. Kurum, tespit edilen suç unsurlarının adli mercilere iletildiğini bildirdi.\n\nKaynak: https://seddk.gov.tr/UploadContent/Documents/Hasar_%C3%87eteleri_Bas%C4%B1n_Duyurusu_20260921_1016.pdf",
                    "/images/news/news-hasar-mucadele.jpg",
                    new DateTime(2026, 9, 21, 10, 0, 0)),

                CreateNews(
                    "Akıllı Eksper Atama Sisteminin İlk Sonuçları Değerlendirildi",
                    "SEDDK ve TOBB heyetleri, eksper bağımsızlığı ile hasar süreçlerini iyileştirmeyi amaçlayan sistemin ilk uygulama sonuçlarını görüştü.",
                    "SEDDK ev sahipliğinde gerçekleştirilen istişare toplantısında sigorta acenteleri ve eksperlerini ilgilendiren güncel sektörel konular ele alındı. Görüşmelerde dijitalleşme, haksız rekabetin önlenmesi ve sahadan gelen talepler değerlendirildi.\n\nSigorta eksperleri gündeminde, 1 Ağustos 2026 itibarıyla Türkiye genelinde uygulanmaya başlanan Akıllı Eksper Atama Sisteminin ilk sonuçları görüşüldü. Toplantıda eksperlerin bağımsızlığının güçlendirilmesi ve hasar süreçlerinin iyileştirilmesine yönelik çalışmalar da ele alındı.\n\nKaynak: https://seddk.gov.tr/tr/seddk-ve-tobb-heyetleri-bir-araya-geldi-1",
                    "/images/news/news-eksper-toplanti.jpg",
                    new DateTime(2026, 8, 7, 10, 0, 0)),

                CreateNews(
                    "IASIU 2026 Konferansı Suistimal Uzmanlarını Bir Araya Getirdi",
                    "Uluslararası Özel İnceleme Birimleri Birliğinin yıllık konferansında güncel suistimal yöntemleri ve araştırma teknikleri ele alındı.",
                    "Uluslararası Özel İnceleme Birimleri Birliği IASIU tarafından düzenlenen 2026 Yıllık Konferansı, 20-23 Eylül tarihlerinde Orlando'da gerçekleştirildi. Program, sigorta suistimalinin tespiti, araştırılması ve önlenmesi alanında çalışan uzmanları bir araya getirdi.\n\nÇalıştaylarda motorlu araç ve mal sigortaları, sağlık suistimalleri, organize dosyalar, teknoloji, sosyal medya araştırmaları, işçi tazminatı ve uluslararası vakalar gibi başlıklar işlendi. Konferans, farklı ülkelerden uzmanların yöntem ve deneyimlerini paylaşmasına imkân sağladı.\n\nKaynak: https://www.iasiu.org/page/2026AnnualConference",
                    "/images/news/news-iasiu-konferans.jpg",
                    new DateTime(2026, 9, 23, 17, 0, 0)),

                CreateNews(
                    "2026 Eksper Yenileme Eğitimleri İçin Başvurular Devam Ediyor",
                    "SEGEM tarafından yürütülen eksper yenileme eğitimleri için son başvuru tarihi 28 Aralık 2026 olarak açıklandı.",
                    "Sigorta Eksperleri Yönetmeliği kapsamında, 1 Ocak 2021 tarihinden önce ruhsat alan veya yenileme eğitimini tamamlayan eksperlerin 2026 yılı yenileme eğitimlerini tamamlaması gerekiyor.\n\nTemel sigortacılık, mühendislik, yangın ve doğal afet, kara araçları, nakliyat, hava-deniz-demiryolu araçları ile emniyeti suistimal gibi farklı uzmanlık dallarında eğitimler bulunuyor. SEGEM duyurusuna göre son başvuru tarihi 28 Aralık, eğitimleri son izleme tarihi ise 31 Aralık 2026 saat 23.59'dur.\n\nKaynak: https://segem.org.tr/duyurular/2026-yili-eksper-yenileme-egitimleri",
                    "/images/news/announcement-eksper-yenileme.jpg",
                    new DateTime(2026, 10, 10, 9, 0, 0),
                    isAnnouncement: true),

                CreateNews(
                    "Yanlış Sigorta Uygulamaları Eğitimi Başvuruları Açıldı",
                    "SEGEM'in e-öğrenme yöntemiyle hazırladığı zorunlu Yanlış Sigorta Uygulamaları Eğitimi için başvurular başladı.",
                    "SEGEM, yanlış sigorta uygulamalarının tespiti, bildirimi, kaydı ve bu uygulamalarla mücadeleye ilişkin 2026 yılı eğitim başvurularını açtı. Eğitim e-öğrenme yöntemiyle gerçekleştirilecek.\n\nİlgili yönetmelik kapsamında sigorta şirketlerinin yönetim kurulu dâhil tüm çalışanlarının eğitime katılması zorunlu tutuluyor. Başvuru yapacak kişilerin SEGEM tarafından yayımlanan başvuru ve uygulama kılavuzunu incelemesi gerekiyor.\n\nKaynak: https://segem.org.tr/duyurular/2026-yili-yanlis-sigorta-uygulamalari-egitimi",
                    "/images/news/announcement-yanlis-uygulamalar.jpg",
                    new DateTime(2026, 10, 9, 9, 0, 0),
                    isAnnouncement: true),

                CreateNews(
                    "Araç Suistimalleri ve Oto Hırsızlığı Eğilimleri Webinarı",
                    "IASIU üyelerine yönelik çevrim içi oturum 4 Kasım 2026 tarihinde gerçekleştirilecek.",
                    "IASIU, araç suistimalleri ve oto hırsızlığındaki güncel eğilimlere odaklanan üyeye özel bir webinar düzenleyeceğini duyurdu. Oturum, 4 Kasım 2026 tarihinde çevrim içi olarak gerçekleştirilecek.\n\nProgramın sigorta suistimali araştırmacıları ve özel inceleme birimi çalışanları için güncel riskleri takip etme ve araştırma yaklaşımlarını geliştirme fırsatı sunması amaçlanıyor. Kayıt ve saat bilgileri IASIU etkinlik takviminde yer alıyor.\n\nKaynak: https://www.iasiu.org/events/event_list.asp",
                    "/images/news/announcement-arac-suistimali.jpg",
                    new DateTime(2026, 10, 8, 9, 0, 0),
                    isAnnouncement: true),

                CreateNews(
                    "Sahte Parçalar ve Hileli Araç Kimlik Numaraları Webinarı",
                    "IASIU'nun sahte otomotiv parçaları, hileli VIN kayıtları ve gizli güvenlik risklerini ele alacağı webinar 2 Aralık'ta yapılacak.",
                    "IASIU tarafından duyurulan çevrim içi oturumda sahte araç parçaları, hileli araç kimlik numaraları ve bunların oluşturduğu güvenlik riskleri ele alınacak. Webinar, 2 Aralık 2026 tarihinde gerçekleştirilecek.\n\nBaşlık özellikle motorlu araç hasar dosyalarında görev yapan araştırmacılar, eksperler ve özel inceleme birimleri açısından önem taşıyor. Katılım ve kayıt ayrıntıları IASIU'nun resmî etkinlik takviminden takip edilebilir.\n\nKaynak: https://www.iasiu.org/events/event_list.asp",
                    "/images/news/announcement-sahte-parca.jpg",
                    new DateTime(2026, 10, 7, 9, 0, 0),
                    isAnnouncement: true)
            };

            var titles = newsItems.Select(item => item.Title).ToList();

            var existingItems = await context.News
                .Where(item => titles.Contains(item.Title))
                .ToListAsync();

            foreach (var newsItem in newsItems)
            {
                var existingItem = existingItems.FirstOrDefault(item =>
                    item.Title == newsItem.Title);

                if (existingItem == null)
                {
                    await context.News.AddAsync(newsItem);
                    continue;
                }

            }

            context.News.Add(new News
            {
                Title = SeedMarkerTitle,
                Summary = "Sistem başlangıç kaydı",
                Content = "Sistem başlangıç kaydı",
                IsActive = false,
                CreatedAt = DateTime.MinValue
            });

            await context.SaveChangesAsync();
        }

        private static News CreateNews(
            string title,
            string summary,
            string content,
            string imageUrl,
            DateTime createdAt,
            bool isAnnouncement = false)
        {
            return new News
            {
                Title = title,
                Summary = summary,
                Content = content,
                ImageUrl = imageUrl,
                CreatedAt = createdAt,
                IsAnnouncement = isAnnouncement,
                IsActive = true
            };
        }
    }
}
