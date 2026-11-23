using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class VolunteerSettings
{
    [Required, StringLength(100)]
    [Display(Name = "Üst Başlık")]
    public string HeroEyebrow { get; set; } = "Birlikte Daha Güçlüyüz";

    [Required, StringLength(200)]
    [Display(Name = "Sayfa Başlığı")]
    public string HeroTitle { get; set; } = "Uzmanlığınızla ELSADER’e Katkı Sağlayın";

    [Required, StringLength(1000)]
    [Display(Name = "Sayfa Açıklaması")]
    public string HeroDescription { get; set; } = "Bilginizi, deneyiminizi ve zamanınızı mesleki dayanışmayı güçlendiren çalışmalara dönüştürün.";

    [Required, StringLength(200)]
    [Display(Name = "Kimler Katılabilir Başlığı")]
    public string WhoCanApplyTitle { get; set; } = "Kimler gönüllü olabilir?";

    [Required, StringLength(2000)]
    [Display(Name = "Kimler Katılabilir Açıklaması")]
    public string WhoCanApplyDescription { get; set; } = "Sigorta, hukuk, saha incelemesi, eğitim, iletişim ve dijital çalışmalar başta olmak üzere farklı alanlarda deneyim sahibi olan veya öğrenmeye istekli herkes gönüllü başvurusu yapabilir.";

    [Required, StringLength(2000)]
    [Display(Name = "Katkı Alanları")]
    public string OpportunityAreas { get; set; } = "Saha İncelemesi\nHukuk ve Mevzuat\nEğitim ve Etkinlik\nDijital İçerik ve İletişim\nBelge ve Veri Analizi\nOrganizasyon Desteği\nDiğer";

    [Required, StringLength(2000)]
    [Display(Name = "Gönüllülüğün Katkıları")]
    public string Benefits { get; set; } = "Mesleki bilgi ve deneyim paylaşımı\nSektör uzmanlarıyla güçlü bir iletişim ağı\nEğitim ve çalışma gruplarına katılım\nToplumsal ve mesleki faydaya katkı";

    [Required, StringLength(2000)]
    [Display(Name = "Başvuru Süreci")]
    public string ProcessSteps { get; set; } = "Başvuru formunu doldurun\nEkibimiz başvurunuzu değerlendirsin\nUygun çalışma alanı için sizinle iletişime geçelim";

    [Required, StringLength(200)]
    [Display(Name = "Ana Sayfa Kart Başlığı")]
    public string HomeCardTitle { get; set; } = "Gönüllü Gücümüze Katılın";

    [Required, StringLength(500)]
    [Display(Name = "Ana Sayfa Kart Açıklaması")]
    public string HomeCardDescription { get; set; } = "Uzmanlığınızla eğitimlere, çalışma gruplarına ve sosyal fayda projelerine destek olun.";

    [Required, StringLength(1000)]
    [Display(Name = "Başarılı Başvuru Mesajı")]
    public string SuccessMessage { get; set; } = "Gönüllü başvurunuz başarıyla alındı. Ekibimiz değerlendirme sonrasında sizinle iletişime geçecektir.";

    [Required, StringLength(2000)]
    [Display(Name = "KVKK Bilgilendirmesi")]
    public string KvkkText { get; set; } = "KVKK Aydınlatma Metni’ni okudum ve gönüllülük başvurum kapsamında kişisel verilerimin işlenmesi hakkında bilgi edindim.";

    [Display(Name = "Başvuru Formu Açık")]
    public bool FormEnabled { get; set; } = true;
}

public class VolunteerApplicationMetadata
{
    public string Status { get; set; } = "Yeni";
    public string? AdminNote { get; set; }
}
