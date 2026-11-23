namespace SigortaDernegi.ViewModels;

public class DuesPaymentReceiptViewModel
{
    public int PaymentId { get; set; }

    public int DuesPeriodId { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public string MemberName { get; set; } = string.Empty;

    public string? RegistryNumber { get; set; }

    public string PeriodName { get; set; } = string.Empty;

    public string PeriodType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "TRY";

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Provider { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTime PaidAtUtc { get; set; }

    public bool IsDemo { get; set; }
}