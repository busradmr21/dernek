using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class ContactMessage
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad soyad alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Adınız Soyadınız")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(150)]
    [Display(Name = "E-posta Adresiniz")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(20)]
    [Display(Name = "Telefon Numaranız")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Konu seçimi zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Konu")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
    [StringLength(2000, MinimumLength = 10,
        ErrorMessage = "Mesajınız 10 ile 2000 karakter arasında olmalıdır.")]
    [Display(Name = "Mesajınız")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "KVKK Bilgilendirmesi")]
    public bool HasReadKvkkNotice { get; set; }

    [StringLength(30)]
    public string Status { get; set; } = "Yeni";

    public int? RecipientMemberProfileId { get; set; }

    public MemberProfile? RecipientMemberProfile { get; set; }

    [StringLength(150)]
    [Display(Name = "Mesajın İletileceği Üye")]
    public string? RecipientDisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
