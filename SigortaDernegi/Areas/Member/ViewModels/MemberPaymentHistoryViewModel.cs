namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberPaymentHistoryViewModel
{
    public int PaymentCount { get; set; }

    public decimal NetPaidAmount { get; set; }

    public decimal RefundedAmount { get; set; }

    public IReadOnlyList<MemberPaymentHistoryItemViewModel> Payments { get; set; }
        = Array.Empty<MemberPaymentHistoryItemViewModel>();
}

public class MemberPaymentHistoryItemViewModel
{
    public int Id { get; set; }

    public string PeriodName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "TRY";

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Provider { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTime PaidAtUtc { get; set; }

    public decimal RefundedAmount { get; set; }

    public bool IsDemo { get; set; }

    public bool CanViewReceipt { get; set; }
}
