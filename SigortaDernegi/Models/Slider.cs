using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty; // Banner başlığı

        [StringLength(250)]
        public string? Description { get; set; } // Kısa açıklama yap

        public string ImageUrl { get; set; } = string.Empty; // Görsel adresi

        public string? ButtonText { get; set; } // Buton yazısı
        public string? ButtonUrl { get; set; } // Butonun gideceği link

        public bool IsActive { get; set; } = true; // Gösterilsin yada gösterilmesin(false)
        public int Order { get; set; } = 0; // Sıralama
    }
}

