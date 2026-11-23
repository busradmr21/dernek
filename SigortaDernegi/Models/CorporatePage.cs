using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class CorporatePage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PageKey { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string MenuTitle { get; set; } = string.Empty;

        [StringLength(100)]
        public string? HeroEyebrow { get; set; }

        [Required]
        [StringLength(200)]
        public string HeroTitle { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? HeroDescription { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }
    }
}