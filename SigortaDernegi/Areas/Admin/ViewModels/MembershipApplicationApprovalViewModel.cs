using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class MembershipApplicationApprovalViewModel
{
    public int ApplicationId { get; set; }

    public string ApplicantName { get; set; } = string.Empty;

    public string MembershipType { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Uzmanlık veya faaliyet alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Uzmanlık / Faaliyet Alanı")]
    public string Specialty { get; set; } = string.Empty;

    [StringLength(40)]
    [Display(Name = "Üye Numarası")]
    public string? RegistryNumber { get; set; }

    [StringLength(80)]
    [Display(Name = "Bölge")]
    public string? Region { get; set; }

    [StringLength(500)]
    [Display(Name = "Kısa Tanıtım")]
    public string? ShortDescription { get; set; }

    public bool HasExistingAccount { get; set; }

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
}
