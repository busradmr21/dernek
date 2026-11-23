using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberProfileViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string MemberType { get; set; } = string.Empty;
    public string? RegistryNumber { get; set; }
    public string LoginEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsPublished { get; set; }
    public DateTime MembershipDate { get; set; }

    [Required(ErrorMessage = "Uzmanlık veya faaliyet alanı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Uzmanlık / Faaliyet Alanı")]
    public string Specialty { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şehir zorunludur.")]
    [StringLength(80)]
    [Display(Name = "Şehir")]
    public string City { get; set; } = string.Empty;

    [StringLength(80)]
    [Display(Name = "Bölge")]
    public string? Region { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası yazmalısınız.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [StringLength(500)]
    [Display(Name = "Kısa Tanıtım")]
    public string? ShortDescription { get; set; }

    [Display(Name = "Profil bilgilerimin uzman ve üye rehberinde yayımlanmasına açık rıza veriyorum")]
    public bool HasPublicationConsent { get; set; }
}
