using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.ViewModels;

public class MemberContactViewModel
{
    public int MemberId { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public string MemberType { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Adınız Soyadınız")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(150)]
    [Display(Name = "E-posta Adresiniz")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konu alanı zorunludur.")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Konu 3 ile 100 karakter arasında olmalıdır.")]
    [Display(Name = "Konu")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
    [StringLength(2000, MinimumLength = 10,
        ErrorMessage = "Mesajınız 10 ile 2000 karakter arasında olmalıdır.")]
    [Display(Name = "Mesajınız")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "KVKK Bilgilendirmesi")]
    public bool HasReadKvkkNotice { get; set; }

    // Otomatik gönderim yapan botların doldurması beklenen görünmez alan.
    public string? Website { get; set; }

    [Required(ErrorMessage = "Güvenlik sorusunu yanıtlayınız.")]
    [Display(Name = "Güvenlik Sorusu")]
    public int? CaptchaAnswer { get; set; }

    public string CaptchaQuestion { get; set; } = string.Empty;

    public string CaptchaToken { get; set; } = string.Empty;
}
