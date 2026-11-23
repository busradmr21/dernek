using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class HomeMissionSettings
{
    [Display(Name = "Bölüm yayında")]
    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Üst başlık zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Üst Başlık")]
    public string Eyebrow { get; set; } = "Misyonumuz & Kuruluş Amacımız";

    [Required(ErrorMessage = "Misyon başlığı zorunludur.")]
    [StringLength(220)]
    [Display(Name = "Misyon Başlığı")]
    public string Title { get; set; } =
        "Bilimsel Doğruluk ve Bağımsız Sigorta Uzmanlığı";

    [Required(ErrorMessage = "Misyon açıklaması zorunludur.")]
    [StringLength(900)]
    [Display(Name = "Misyon Açıklaması")]
    public string Description { get; set; } =
        "Sigorta uyuşmazlıklarında bağımsız bilimsel inceleme ve adli kaza analizleriyle haksız hasar suiistimallerini (fraud) önlemek amacıyla kurulmuştur.";

    [Required]
    [StringLength(120)]
    [Display(Name = "1. İlke Başlığı")]
    public string ResearchTitle { get; set; } = "Bağımsız Bilimsel Araştırma";

    [Required]
    [StringLength(420)]
    [Display(Name = "1. İlke Açıklaması")]
    public string ResearchDescription { get; set; } =
        "Adli kaza rekonstrüksiyonu, teknik veri analizi ve aktüeryal doğrulama.";

    [Required]
    [StringLength(120)]
    [Display(Name = "2. İlke Başlığı")]
    public string FraudTitle { get; set; } = "Haksız Hasar (Fraud) Tespiti";

    [Required]
    [StringLength(420)]
    [Display(Name = "2. İlke Açıklaması")]
    public string FraudDescription { get; set; } =
        "Suiistimalleri önleme, dijital dosya inceleme ve hasar risk analizleri.";

    [Required]
    [StringLength(120)]
    [Display(Name = "3. İlke Başlığı")]
    public string EthicsTitle { get; set; } = "Adli Tahkim & Etik Standartlar";

    [Required]
    [StringLength(420)]
    [Display(Name = "3. İlke Açıklaması")]
    public string EthicsDescription { get; set; } =
        "Yargı ve Sigorta Tahkim süreçlerinde tarafsız ve bağımsız bilirkişilik.";

    [Required]
    [StringLength(320)]
    [Display(Name = "Alt Bilgi")]
    public string LegalReference { get; set; } =
        "5684 Sayılı Sigortacılık Kanunu Çerçevesinde Bağımsız Uzmanlık";
}
