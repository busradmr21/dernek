using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class DuesPeriod
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string PeriodType { get; set; } = DuesPeriodTypes.Annual;

    public decimal Amount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "TRY";

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DueDate { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    public ICollection<MemberDue> MemberDues { get; set; }
        = new List<MemberDue>();
}

public static class DuesPeriodTypes
{
    public const string Monthly = "Aylık";
    public const string Quarterly = "3 Aylık";
    public const string SemiAnnual = "6 Aylık";
    public const string Annual = "Yıllık";
    public const string OneTime = "Tek Seferlik";

    public static readonly string[] All =
        [Monthly, Quarterly, SemiAnnual, Annual, OneTime];
}
