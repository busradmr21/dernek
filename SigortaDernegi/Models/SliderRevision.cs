using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models
{
    public class SliderRevision
    {
        public int Id { get; set; }

        public int SliderId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(60)]
        public string? ButtonText { get; set; }

        [StringLength(500)]
        public string? ButtonUrl { get; set; }

        public bool IsActive { get; set; }

        public int Order { get; set; }

        [Required]
        [StringLength(50)]
        public string ChangeType { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [StringLength(256)]
        public string? ChangedBy { get; set; }
    }
}