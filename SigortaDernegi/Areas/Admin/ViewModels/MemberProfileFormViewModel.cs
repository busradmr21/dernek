using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class MemberProfileFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Üye türünü seçmelisiniz.")]
    [StringLength(30)]
    [Display(Name = "Üye Türü")]
    public string MemberType { get; set; } = "Bireysel";

    [Required(ErrorMessage = "Üye veya kurum adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Üye / Kurum Adı")]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(40)]
    [Display(Name = "Üye Numarası")]
    public string? RegistryNumber { get; set; }

    [Required(ErrorMessage = "Uzmanlık veya faaliyet alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Uzmanlık / Faaliyet Alanı")]
    public string Specialty { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şehir bilgisi zorunludur.")]
    [StringLength(80)]
    [Display(Name = "Şehir")]
    public string City { get; set; } = string.Empty;

    [StringLength(80)]
    [Display(Name = "Bölge")]
    public string? Region { get; set; }

    [StringLength(500)]
    [Display(Name = "Kısa Tanıtım")]
    public string? ShortDescription { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir giriş e-posta adresi yazmalısınız.")]
    [StringLength(256)]
    [Display(Name = "Giriş E-postası")]
    public string? LoginEmail { get; set; }

    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage = "Geçici şifre en az 8 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Geçici Şifre")]
    public string? TemporaryPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare(
        nameof(TemporaryPassword),
        ErrorMessage = "Yazılan şifreler aynı değil.")]
    [Display(Name = "Geçici Şifre Tekrarı")]
    public string? ConfirmPassword { get; set; }

    [Display(Name = "Faal Üye")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Yayın İzni Var")]
    public bool HasPublicationConsent { get; set; }

    [Display(Name = "Uzman / Üye Rehberinde Yayımla")]
    public bool IsPublished { get; set; }
}
