using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class AssociationEvent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Etkinlik Başlığı")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Kısa Açıklama")]
        public string Summary { get; set; } = string.Empty;

        [StringLength(4000)]
        [Display(Name = "Etkinlik Detayı")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Etkinlik Türü")]
        public string? EventType { get; set; }

        [StringLength(200)]
        [Display(Name = "Konum")]
        public string? Location { get; set; }

        [StringLength(500)]
        [Display(Name = "Görsel Adresi")]
        public string? ImageUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Kayıt Bağlantısı")]
        public string? RegistrationUrl { get; set; }

        [Display(Name = "Çevrim İçi Etkinlik")]
        public bool IsOnline { get; set; }

        [Display(Name = "Öne Çıkan Etkinlik")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Yayında")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}