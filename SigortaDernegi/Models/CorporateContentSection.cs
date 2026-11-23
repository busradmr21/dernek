using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class CorporateContentSection
    {
        public int Id { get; set; }

        public int CorporatePageId { get; set; }

        [Required]
        [StringLength(80)]
        public string SectionKey { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Eyebrow { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? ContentHtml { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? UpdatedBy { get; set; }

        public CorporatePage? CorporatePage { get; set; }
    }
}
