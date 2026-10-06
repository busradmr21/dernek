using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; // Haber başlığı 

        [Required]
        public string Summary { get; set; } = string.Empty; // Ana sayfadaki kısa özet

        [Required]
        public string Content { get; set; } = string.Empty; // Haberin detayları

        public string? ImageUrl { get; set; } // Görsel

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Yayın tarihi

        public bool IsAnnouncement { get; set; } = false; // Duyuru mu yoksa haber mi
        public bool IsActive { get; set; } = true;
    }
}
