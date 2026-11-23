using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class MembershipApplication
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Üyelik türünü seçiniz.")]
        [StringLength(30)]
        [Display(Name = "Üyelik Türü")]
        public string MembershipType { get; set; } = "Bireysel";

        [Required(ErrorMessage = "Ad soyad veya kurum adı zorunludur.")]
        [StringLength(150)]
        [Display(Name = "Ad Soyad / Kurum Adı")]
        public string ApplicantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(150)]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [StringLength(20)]
        [Display(Name = "Telefon Numarası")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(120)]
        [Display(Name = "Uzmanlık / Faaliyet Alanı")]
        public string? Specialty { get; set; }

        [Required(ErrorMessage = "Şehir bilgisi zorunludur.")]
        [StringLength(80)]
        [Display(Name = "Şehir")]
        public string City { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Kısa Açıklama")]
        public string? Message { get; set; }

        [Display(Name = "Tüzük ve Etik İlkeler")]
        public bool HasAcceptedBylaws { get; set; }

        [Display(Name = "KVKK Bilgilendirmesi")]
        public bool HasReadKvkkNotice { get; set; }

        [StringLength(30)]
        public string ApplicationStatus { get; set; } =
            MembershipApplicationStatuses.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAtUtc { get; set; }

        [StringLength(256)]
        public string? ReviewedBy { get; set; }

        public MemberProfile? MemberProfile { get; set; }
    }
}
