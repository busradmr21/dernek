using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class ConfidentialReport
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string InsuranceBranch { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Category { get; set; } = string.Empty;

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(160)]
    public string? Location { get; set; }

    public DateTime? IncidentDate { get; set; }

    [StringLength(180)]
    public string? ReferenceInformation { get; set; }

    [Required, StringLength(4000)]
    public string Details { get; set; } = string.Empty;

    public bool WantsContact { get; set; }

    [StringLength(120)]
    public string? ReporterName { get; set; }

    [StringLength(150)]
    public string? ReporterEmail { get; set; }

    [StringLength(30)]
    public string? ReporterPhone { get; set; }

    [Required, StringLength(30)]
    public string Status { get; set; } = "Yeni";

    [StringLength(2000)]
    public string? AdminNote { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<ConfidentialReportAttachment> Attachments { get; set; } =
        new List<ConfidentialReportAttachment>();
}

public class ConfidentialReportAttachment
{
    public int Id { get; set; }

    public int ConfidentialReportId { get; set; }

    public ConfidentialReport ConfidentialReport { get; set; } = null!;

    [Required, StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string ContentType { get; set; } = "application/octet-stream";

    public long FileSize { get; set; }

    [Required]
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
