using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.ViewModels;

public class HomeContactFormViewModel
{
    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(150)]
    [Display(Name = "E-Posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başvuru türü seçmelisiniz.")]
    [StringLength(100)]
    [Display(Name = "Konu / Başvuru Türü")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
    [StringLength(2000, MinimumLength = 10,
        ErrorMessage = "Mesajınız 10 ile 2000 karakter arasında olmalıdır.")]
    [Display(Name = "Mesajınız")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "KVKK bilgilendirmesini okudum.")]
    public bool HasReadKvkkNotice { get; set; }
}

public class ConfidentialReportFormViewModel
{
    [Required(ErrorMessage = "Sigorta branşı seçmelisiniz.")]
    [StringLength(100)]
    [Display(Name = "Sigorta Branşı")]
    public string InsuranceBranch { get; set; } = string.Empty;

    [Required(ErrorMessage = "İhbar kategorisi seçmelisiniz.")]
    [StringLength(120)]
    [Display(Name = "Olay / Suistimal Kategorisi")]
    public string Category { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Şehir / Bölge")]
    public string? City { get; set; }

    [StringLength(160)]
    [Display(Name = "İlçe / Konum")]
    public string? Location { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Olay Tarihi")]
    public DateTime? IncidentDate { get; set; }

    [StringLength(180)]
    [Display(Name = "Plaka / Dosya / Referans No")]
    public string? ReferenceInformation { get; set; }

    [Required(ErrorMessage = "İhbar detayını yazmalısınız.")]
    [StringLength(4000, MinimumLength = 20,
        ErrorMessage = "İhbar detayı 20 ile 4000 karakter arasında olmalıdır.")]
    [Display(Name = "İhbar Detayı ve Vaka Açıklaması")]
    public string Details { get; set; } = string.Empty;

    public bool WantsContact { get; set; }

    [StringLength(120)]
    [Display(Name = "Ad Soyad")]
    public string? ReporterName { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(150)]
    [Display(Name = "E-Posta")]
    public string? ReporterEmail { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(30)]
    [Display(Name = "Telefon")]
    public string? ReporterPhone { get; set; }
}

public class ConfidentialReportPageViewModel
{
    public ConfidentialReportFormViewModel ReportForm { get; set; } = new();
}
