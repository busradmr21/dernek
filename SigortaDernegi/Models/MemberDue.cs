using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class MemberDue
{
    public int Id { get; set; }

    public int DuesPeriodId { get; set; }
    public DuesPeriod DuesPeriod { get; set; } = null!;

    public int? MemberProfileId { get; set; }
    public MemberProfile? MemberProfile { get; set; }

    public int? BoardMemberId { get; set; }
    public BoardMember? BoardMember { get; set; }

    public decimal Amount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "TRY";

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = MemberDueStatuses.Unpaid;

    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAtUtc { get; set; }

    [StringLength(500)]
    public string? AdminNote { get; set; }

    public ICollection<DuesPayment> Payments { get; set; }
        = new List<DuesPayment>();
}

public static class MemberDueStatuses
{
    public const string Unpaid = "Ödenmedi";
    public const string Paid = "Ödendi";
    public const string Exempt = "Muaf";
    public const string Cancelled = "İptal";
}
