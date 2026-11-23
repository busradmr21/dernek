using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Models;

public class DuesPayment
{
    public int Id { get; set; }

    public int MemberDueId { get; set; }
    public MemberDue MemberDue { get; set; } = null!;

    public decimal Amount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "TRY";

    [Required]
    [StringLength(30)]
    public string PaymentMethod { get; set; } = DuesPaymentMethods.BankTransfer;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = DuesPaymentStatuses.Completed;

    [StringLength(50)]
    public string? Provider { get; set; }

    [StringLength(150)]
    public string? ProviderPaymentId { get; set; }

    [StringLength(120)]
    public string? ReferenceNumber { get; set; }

    public DateTime PaidAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(500)]
    public string? AdminNote { get; set; }

    public decimal RefundedAmount { get; set; }
    public DateTime? RefundedAtUtc { get; set; }
}

public static class DuesPaymentMethods
{
    public const string BankTransfer = "Havale / EFT";
    public const string Cash = "Nakit";
    public const string OnlineCard = "Online Kart";
    public const string Other = "Diğer";

    public static readonly string[] ManualMethods =
        [BankTransfer, Cash, Other];
}

public static class DuesPaymentStatuses
{
    public const string Pending = "Bekliyor";
    public const string Completed = "Başarılı";
    public const string Failed = "Başarısız";
    public const string Refunded = "İade Edildi";
    public const string PartiallyRefunded = "Kısmi İade";
}
