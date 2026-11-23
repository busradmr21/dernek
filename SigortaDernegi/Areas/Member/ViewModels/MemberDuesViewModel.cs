namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberDuesViewModel
{
    public decimal OutstandingAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public int UnpaidCount { get; set; }
    public int OverdueCount { get; set; }
    public IReadOnlyList<MemberDueItemViewModel> Items { get; set; }
        = Array.Empty<MemberDueItemViewModel>();
}

public class MemberDueItemViewModel
{
    public int Id { get; set; }
    public int? PaymentId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public string PeriodType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
}
