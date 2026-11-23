using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class MemberProfile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Üye Türü")]
        public string MemberType { get; set; } = "Bireysel";

        [Required]
        [StringLength(150)]
        [Display(Name = "Üye / Kurum Adı")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(40)]
        [Display(Name = "Üye Numarası")]
        public string? RegistryNumber { get; set; }

        [Required]
        [StringLength(120)]
        [Display(Name = "Uzmanlık / Faaliyet Alanı")]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        [Display(Name = "Şehir")]
        public string City { get; set; } = string.Empty;

        [StringLength(80)]
        [Display(Name = "Bölge")]
        public string? Region { get; set; }

        [StringLength(500)]
        [Display(Name = "Kısa Tanıtım")]
        public string? ShortDescription { get; set; }

        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Rehber İletişim E-postası")]
        public string? PublicEmail { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "İletişim Telefonu")]
        public string? Phone { get; set; }

        [StringLength(450)]
        public string? IdentityUserId { get; set; }

        public Microsoft.AspNetCore.Identity.IdentityUser? IdentityUser { get; set; }

        public int? MembershipApplicationId { get; set; }

        public MembershipApplication? MembershipApplication { get; set; }

        [Display(Name = "Faal Üye")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Yayın İzni Var")]
        public bool HasPublicationConsent { get; set; } = false;

        [Display(Name = "Rehberde Yayımla")]
        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? MembershipApprovedAtUtc { get; set; }
    }
}
