namespace SigortaDernegi.Areas.Admin.ViewModels;

public class DuesPaymentHistoryViewModel
{
    public string? Search { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public int PaymentCount { get; set; }

    public decimal TotalCollected { get; set; }

    public IReadOnlyList<DuesPaymentHistoryItemViewModel> Payments { get; set; }
        = Array.Empty<DuesPaymentHistoryItemViewModel>();
}

public class DuesPaymentHistoryItemViewModel
{
    public int Id { get; set; }

    public string MemberName { get; set; } = string.Empty;

    public string? RegistryNumber { get; set; }

    public string PeriodName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Provider { get; set; }

    public string? ReferenceNumber { get; set; }

    public DateTime PaidAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public bool IsDemo { get; set; }
}