using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SigortaDernegi.Controllers
{
    // Haber Model Sınıfı
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Detay metni
        public string Category { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = string.Empty;
    }

    public class NewsController : Controller
    {
        // Örnek Veri Listesi
        private static readonly List<NewsItem> NewsData = new List<NewsItem>
        {
            new NewsItem
            {
                Id = 1,
                Title = "Saha İncelemelerinde KVKK ve Dijital Delil Standartları",
                Summary = "Kişisel Verilerin Korunması Kanunu uyarınca araştırmacıların sahada elde ettiği dijital delil, fotoğraf ve kaza yeri incelemelerine dair yeni uygulama rehberi yayınlandı.",
                Content = @"Kişisel Verilerin Korunması Kanunu (KVKK) kapsamında sigorta araştırmacılığı ve saha denetimleri yeniden yapılandırılıyor. 

Yeni yayınlanan uygulama rehberine göre:
1. **Rıza ve Aydınlatma Metinleri:** Saha araştırmalarında kişilerden alınan beyan ve görsellerde aydınlatma yükümlülüğünün eksiksiz yerine getirilmesi şart koşulmuştur.
2. **Dijital Delil Güvenliği:** Fotoğraf, video ve ses kayıtlarının uçtan uca şifreli ortamlarda saklanması ve sadece yetkili hasar uzmanları ile paylaşılması zorunlu hale getirilmiştir.
3. **Kaza Yeri İncelemeleri:** Üçüncü şahıslara ait kişisel verilerin (araç plakaları, yüzler vb.) maskelenmesi hususunda teknik altyapı standartları belirlenmiştir.

Tüm üyelerimizin saha çalışmalarında bu rehbere uygun hareket etmesi önemle rica olunur.",
                Category = "Mevzuat",
                Date = "05 Ekim 2026",
                Author = "Hukuk İşleri Komisyonu",
                BadgeClass = "bg-warning text-dark"
            },
            new NewsItem
            {
                Id = 2,
                Title = "Organize Hasar Suistimalleri ve Teknik İnceleme Semineri",
                Summary = "Derneğimiz bünyesinde düzenlenecek olan 'Kurgusal Kazaların Tespiti, Modül Analizi ve Kriminoloji' eğitimi başvuruları başladı.",
                Content = @"Derneğimizin Eğitim Komitesi tarafından organize edilen 'Organize Hasar Suistimalleri ve Teknik İnceleme Semineri' önümüzdeki ay gerçekleştirilecektir.

**Seminer Konu Başlıkları:**
* Araç Beyin (ECU) verilerinden kaza anı hız ve darbe analizi yapma,
* Kurgusal trafik kazalarında boya, iz ve deformasyon uyumsuzluklarının tespiti,
* Yangın ve ağır hasar dosyalarında kriminolojik inceleme teknikleri,
* Yapay zeka destekli suistimal tespit yazılımlarının kullanımı.

Seminer sonunda başarılı olan katılımcılara **'Saha İnceleme Uzmanlık Sertifikası'** verilecektir. Kontenjan 50 kişi ile sınırlıdır.",
                Category = "Eğitim",
                Date = "28 Eylül 2026",
                Author = "Eğitim Komitesi",
                BadgeClass = "bg-danger"
            },
            new NewsItem
            {
                Id = 3,
                Title = "2026 Yılı Olağan Genel Kurul Toplantı Çağrısı",
                Summary = "Sigorta Araştırmacıları ve Uzmanları Derneği 2026 yılı Olağan Genel Kurul Toplantısı gündem maddeleri ve katılım şartları duyurulmuştur.",
                Content = @"Sigorta Araştırmacıları ve Uzmanları Derneği Yönetim Kurulu'ndan Duyurudur:

Derneğimizin 2026 Yılı Olağan Genel Kurul Toplantısı **15 Kasım 2026 Pazar günü saat 10:00'da** Genel Merkez Konferans Salonu'nda yapılacaktır.

**Gündem Maddeleri:**
1. Açılış, saygı duruşu ve İstiklal Marşı,
2. Başkanlık Divanı seçimi,
3. 2025-2026 dönemi Yönetim Kurulu faaliyet ve finansal raporlarının okunması,
4. Yönetim ve Denetim Kurullarının ibrası,
5. Yeni dönem Yönetim ve Denetim Kurulu üyelerinin seçimi,
6. Dilek ve temenniler, kapanış.

Çoğunluk sağlanamadığı takdirde ikinci toplantı **22 Kasım 2026** tarihinde aynı yer ve saatte çoğunluk aranmaksızın yapılacaktır.",
                Category = "Duyuru",
                Date = "15 Eylül 2026",
                Author = "Yönetim Kurulu",
                BadgeClass = "bg-primary"
            },
            new NewsItem
            {
                Id = 4,
                Title = "Yargıtay'dan Sahte Kaza Raporlarına İlişkin Emsal Karar",
                Summary = "Yargıtay 17. Hukuk Dairesi, kurgusal trafik kazalarında sigorta şirketini yanıltmaya çalışan taraflar hakkında nitelikli dolandırıcılık hükmünü onadı.",
                Content = @"Yargıtay, sigorta sektörünü yakından ilgilendiren emsal nitelikte bir karara imza attı.

Dava konusu olayda, önceden hasarlı olan iki aracı anlaşmalı olarak kaza yapmış gibi gösterip sigorta şirketinden tazminat talep eden şahıslar hakkında açılan davada yerel mahkeme mahkumiyet kararı vermişti.

Temyiz incelemesini tamamlayan Yargıtay 17. Hukuk Dairesi:
* Sahte tutanak düzenlemenin ve kurgusal kaza ortamı hazırlamanın **'TCK 158/1-k maddesi uyarınca Sigorta Şirketlerinin Dolandırılması'** suçunu oluşturduğuna,
* Suçun teşebbüs aşamasında kalmış olsa dahi adli para ve hapis cezası gerektirdiğine oy birliğiyle karar verdi.

Bu karar, saha araştırmacılarının topladığı delillerin mahkemeler nezdindeki önemini bir kez daha ortaya koymuştur.",
                Category = "Yargıtay Kararı",
                Date = "02 Eylül 2026",
                Author = "Mevzuat Grubu",
                BadgeClass = "bg-dark"
            }
        };

        // Liste Sayfası
        public IActionResult Index()
        {
            return View(NewsData);
        }

        // Detay Sayfası
        public IActionResult Detail(int id)
        {
            var news = NewsData.FirstOrDefault(x => x.Id == id);
            if (news == null)
            {
                return RedirectToAction("Index");
            }

            return View(news);
        }
    }
}
