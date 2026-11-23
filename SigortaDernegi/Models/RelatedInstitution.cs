using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class RelatedInstitution
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Abbreviation { get; set; } = string.Empty;

    [Required]
    [StringLength(180)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string WebsiteUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
