using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data
{
    public static class EventSeedData
    {
        private const string SeedMarkerTitle = "__event_seed_complete";

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.AssociationEvents.AnyAsync(item => item.Title == SeedMarkerTitle))
            {
                return;
            }

            var events = new List<AssociationEvent>
            {
                CreateEvent(
                    "Afet Sonrası Hasar Araştırmalarında Suistimal Göstergeleri",
                    "Afet dönemlerinde karşılaşılan olağan dışı hasar bildirimleri ve araştırma yöntemleri ele alındı.",
                    "Seminerde afet sonrasında açılan hasar dosyalarında dikkat edilmesi gereken suistimal göstergeleri incelendi.\n\nKatılımcılar, saha bulgularının kayda alınması ve kurumlar arası bilgi paylaşımı hakkında örnek çalışmalar yaptı.",
                    new DateTime(2026, 6, 18, 10, 0, 0),
                    new DateTime(2026, 6, 18, 16, 0, 0),
                    "Seminer", "İzmir", "/images/events/event-seminer.jpg"),

                CreateEvent(
                    "Dijital Deliller ve Uzaktan Hasar İnceleme Teknikleri",
                    "Dijital kayıtların doğrulanması ve uzaktan inceleme süreçlerinde delil bütünlüğünün korunması anlatıldı.",
                    "Webinarda fotoğraf, video, konum ve zaman verilerinin güvenilirliği ele alındı.\n\nUzaktan hasar incelemesinde kullanılabilecek kontrol adımları ve raporlama ilkeleri örneklerle açıklandı.",
                    new DateTime(2026, 7, 23, 19, 30, 0),
                    new DateTime(2026, 7, 23, 21, 0, 0),
                    "Webinar", "Çevrim içi", "/images/events/event-webinar.jpg",
                    isOnline: true),

                CreateEvent(
                    "Araştırma Görüşmelerinde Mülakat ve Beyan Analizi",
                    "Görüşme planlama, doğru soru hazırlama ve çelişkili beyanları değerlendirme yöntemleri uygulandı.",
                    "Eğitimde araştırma görüşmesinin hazırlık, uygulama ve raporlama aşamaları işlendi.\n\nKatılımcılar örnek senaryolar üzerinden açık uçlu soru sorma, aktif dinleme ve beyan karşılaştırma çalışmaları yaptı.",
                    new DateTime(2026, 8, 13, 9, 30, 0),
                    new DateTime(2026, 8, 14, 17, 0, 0),
                    "Eğitim", "Bursa", "/images/events/event-egitim.jpg"),

                CreateEvent(
                    "Sigorta Araştırmacılığında Etik Sınırlar",
                    "Mesleki bağımsızlık, tarafsızlık, gizlilik ve kişisel verilerin korunması konuşuldu.",
                    "Söyleşide araştırmacının yetki ve sorumluluk sınırları farklı meslek gruplarının bakış açısıyla değerlendirildi.\n\nAnonimleştirilmiş örnek olaylar üzerinden etik ikilemler ve doğru yaklaşım yöntemleri tartışıldı.",
                    new DateTime(2026, 9, 17, 14, 0, 0),
                    new DateTime(2026, 9, 17, 17, 0, 0),
                    "Söyleşi", "İstanbul", "/images/events/event-soylesi.jpg"),

                CreateEvent(
                    "Hasar İncelemelerinde Delil Toplama ve Raporlama Standartları",
                    "Sigorta araştırmalarında delillerin korunması, kayıt altına alınması ve tarafsız rapor hazırlanması ele alınıyor.",
                    "Seminerde fiziksel ve dijital delillerin doğru şekilde toplanması, saklanması ve kayıt altına alınması anlatılmaktadır.\n\nKatılımcılar örnek vakalar üzerinden delil bütünlüğü, tutanak hazırlama ve savunulabilir araştırma raporu oluşturma yöntemlerini incelemektedir.",
                    new DateTime(2026, 10, 10, 9, 0, 0),
                    new DateTime(2026, 10, 12, 18, 0, 0),
                    "Seminer", "İstanbul", "/images/events/event-seminer.jpg"),

                CreateEvent(
                    "Araç Suistimalleri ve Güncel Oto Hırsızlığı Yöntemleri",
                    "Planlı kazalar, araç kimlik bilgilerindeki değişiklikler ve şüpheli hasar göstergeleri inceleniyor.",
                    "Webinarda güncel araç suistimali yöntemleri, planlı kaza belirtileri ve araç kimlik bilgilerinde yapılan müdahaleler ele alınmaktadır.\n\nŞüpheli hasar dosyalarının değerlendirilmesinde kullanılabilecek temel kontrol noktaları örneklerle açıklanmaktadır.",
                    new DateTime(2026, 10, 11, 9, 0, 0),
                    new DateTime(2026, 10, 12, 21, 0, 0),
                    "Webinar", "Çevrim içi", "/images/events/event-webinar.jpg",
                    isOnline: true),

                CreateEvent(
                    "Otomotiv Hasar İncelemeleri ve Adli Kaza Analizi",
                    "Araç deformasyonu, hasar-beyan uyumu ve kaza senaryoları teknik olarak değerlendiriliyor.",
                    "Eğitimde araç üzerindeki hasar izlerinin değerlendirilmesi, mekanik deformasyonların incelenmesi ve beyan edilen kaza ile mevcut hasarın karşılaştırılması ele alınmaktadır.\n\nKatılımcılar örnek dosyalar üzerinden teknik inceleme ve raporlama uygulamaları gerçekleştirmektedir.",
                    new DateTime(2026, 10, 9, 9, 30, 0),
                    new DateTime(2026, 10, 13, 17, 0, 0),
                    "Eğitim", "Ankara", "/images/events/event-egitim.jpg"),

                CreateEvent(
                    "Bir Hasar Dosyasının Sahadan Mahkemeye Yolculuğu",
                    "Araştırmacı, eksper ve hukukçuların hasar dosyasındaki görevleri mesleki deneyimler üzerinden konuşuluyor.",
                    "Söyleşide bir hasar dosyasının ilk bildirimden saha incelemesine, uzman değerlendirmesinden hukuki sürece kadar ilerleyişi ele alınmaktadır.\n\nAraştırmacı, eksper ve hukukçu bakış açılarıyla görev sınırları, iş birliği ve etik sorumluluklar değerlendirilmektedir.",
                    new DateTime(2026, 10, 11, 10, 0, 0),
                    new DateTime(2026, 10, 12, 18, 0, 0),
                    "Söyleşi", "İstanbul", "/images/events/event-soylesi.jpg"),

                CreateEvent(
                    "Sigorta İncelemelerinde Hukuki Süreç ve Raporun Delil Niteliği",
                    "Araştırma raporlarının hukuki süreçlerdeki yeri ve savunulabilir rapor hazırlama esasları ele alınacaktır.",
                    "Seminerde araştırma raporlarının yargı süreçlerindeki kullanım alanları ve raporun taşıması gereken temel nitelikler anlatılacaktır.\n\nÖrnek dosyalar üzerinden tarafsızlık, kaynak gösterme ve bulguların açık biçimde sunulması değerlendirilecektir.",
                    new DateTime(2026, 11, 14, 10, 0, 0),
                    new DateTime(2026, 11, 14, 16, 30, 0),
                    "Seminer", "Ankara", "/images/events/event-seminer.jpg",
                    isFeatured: true),

                CreateEvent(
                    "Uzaktan Gözetim, Açık Kaynak Araştırması ve Veri Doğrulama",
                    "Açık kaynaklardan elde edilen verilerin doğrulanması ve hukuka uygun biçimde raporlanması anlatılacaktır.",
                    "Webinarda açık kaynak araştırmasının sınırları, dijital içerik doğrulama yöntemleri ve kayıtların delil değerinin korunması ele alınacaktır.\n\nKatılımcılara uygulanabilir bir kontrol listesi sunulacaktır.",
                    new DateTime(2026, 11, 28, 19, 30, 0),
                    new DateTime(2026, 11, 28, 21, 0, 0),
                    "Webinar", "Çevrim içi", "/images/events/event-webinar.jpg",
                    isOnline: true),

                CreateEvent(
                    "Sigorta Suistimali Tespiti ve Vaka Raporlama Atölyesi",
                    "Şüpheli dosya göstergelerinin belirlenmesi ve bulguların düzenli bir vaka raporuna dönüştürülmesi uygulanacaktır.",
                    "Eğitimde suistimal göstergeleri, dosya önceliklendirme ve inceleme planı hazırlama yöntemleri işlenecektir.\n\nKatılımcılar örnek bir dosya üzerinden bulgu tablosu ve sonuç raporu hazırlayacaktır.",
                    new DateTime(2026, 12, 5, 9, 30, 0),
                    new DateTime(2026, 12, 6, 17, 0, 0),
                    "Eğitim", "İstanbul", "/images/events/event-egitim.jpg"),

                CreateEvent(
                    "Sigorta Araştırmalarında Hukuk, Etik ve KVKK Uygulamaları",
                    "Araştırma süreçlerinde hukuka uygun veri işleme, gizlilik ve etik karar verme konuları işlenecektir.",
                    "Eğitimde kişisel verilerin işlenmesi, saklanması ve paylaşılması sırasında dikkat edilmesi gereken temel ilkeler ele alınacaktır.\n\nVaka çalışmalarıyla etik karar verme ve kayıt güvenliği uygulamaları yapılacaktır.",
                    new DateTime(2027, 1, 16, 9, 30, 0),
                    new DateTime(2027, 1, 17, 17, 0, 0),
                    "Eğitim", "İzmir", "/images/events/event-egitim.jpg"),

                CreateEvent(
                    "Yapay Zekâ ve Veri Analizi Sigorta Araştırmalarını Nasıl Değiştiriyor?",
                    "Yapay zekâ destekli inceleme araçlarının fırsatları, sınırları ve etik riskleri uzmanlarla konuşulacaktır.",
                    "Söyleşide veri analizi ve yapay zekânın şüpheli dosyaların belirlenmesindeki rolü tartışılacaktır.\n\nOtomatik kararların denetlenmesi, insan değerlendirmesinin önemi ve veri kalitesi konuları farklı bakış açılarıyla ele alınacaktır.",
                    new DateTime(2027, 2, 13, 14, 0, 0),
                    new DateTime(2027, 2, 13, 17, 0, 0),
                    "Söyleşi", "Çevrim içi", "/images/events/event-soylesi.jpg",
                    isOnline: true)
            };

            var titles = events.Select(eventItem => eventItem.Title).ToList();

            var existingEvents = await context.AssociationEvents
                .Where(eventItem => titles.Contains(eventItem.Title))
                .ToListAsync();

            foreach (var eventItem in events)
            {
                var existingEvent = existingEvents.FirstOrDefault(existing =>
                    existing.Title == eventItem.Title);

                if (existingEvent == null)
                {
                    await context.AssociationEvents.AddAsync(eventItem);
                    continue;
                }

            }

            context.AssociationEvents.Add(new AssociationEvent
            {
                Title = SeedMarkerTitle,
                Summary = "Sistem başlangıç kaydı",
                StartDate = DateTime.MinValue,
                IsActive = false,
                CreatedAt = DateTime.MinValue
            });

            await context.SaveChangesAsync();
        }

        private static AssociationEvent CreateEvent(
            string title,
            string summary,
            string description,
            DateTime startDate,
            DateTime endDate,
            string eventType,
            string location,
            string imageUrl,
            bool isOnline = false,
            bool isFeatured = false)
        {
            return new AssociationEvent
            {
                Title = title,
                Summary = summary,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                EventType = eventType,
                Location = location,
                ImageUrl = imageUrl,
                IsOnline = isOnline,
                IsFeatured = isFeatured,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
