using System.ComponentModel.DataAnnotations;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class DuesManagementIndexViewModel
{
    public IReadOnlyList<DuesPeriodListItemViewModel> Periods { get; set; }
        = Array.Empty<DuesPeriodListItemViewModel>();

    public int PeriodCount { get; set; }
    public int ActivePeriodCount { get; set; }
    public decimal TotalReceivable { get; set; }
    public decimal CollectedAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
}

public class DuesPeriodListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PeriodType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsActive { get; set; }
    public int AssignedMemberCount { get; set; }
    public int PaidMemberCount { get; set; }
    public decimal CollectedAmount { get; set; }
}

public class DuesPeriodFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dönem adı zorunludur.")]
    [StringLength(120)]
    [Display(Name = "Aidat Dönemi Adı")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dönem türünü seçmelisiniz.")]
    [StringLength(30)]
    [Display(Name = "Dönem Türü")]
    public string PeriodType { get; set; } = DuesPeriodTypes.Annual;

    [Range(0.01, 99999999, ErrorMessage = "Aidat tutarı sıfırdan büyük olmalıdır.")]
    [Display(Name = "Üyenin Ödeyeceği Sabit Tutar")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Dönem Başlangıcı")]
    public DateTime StartDate { get; set; } = new(DateTime.Today.Year, 1, 1);

    [DataType(DataType.Date)]
    [Display(Name = "Dönem Bitişi")]
    public DateTime EndDate { get; set; } = new(DateTime.Today.Year, 12, 31);

    [DataType(DataType.Date)]
    [Display(Name = "Son Ödeme Tarihi")]
    public DateTime DueDate { get; set; } = new(DateTime.Today.Year, 3, 31);

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    [Display(Name = "Dönem Aktif")]
    public bool IsActive { get; set; } = true;
}

public class DuesPeriodDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PeriodType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int ActiveMemberCount { get; set; }
    public int AssignedMemberCount { get; set; }
    public int PaidMemberCount { get; set; }
    public decimal CollectedAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public bool DemoResetEnabled { get; set; }
    public IReadOnlyList<MemberDueAdminItemViewModel> MemberDues { get; set; }
        = Array.Empty<MemberDueAdminItemViewModel>();
}

public class MemberDueAdminItemViewModel
{
    public int Id { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public string? RegistryNumber { get; set; }
    public string MemberType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public bool IsOverdue { get; set; }
    public bool HasDemoPayment { get; set; }
}

public class ManualDuesPaymentViewModel
{
    public int MemberDueId { get; set; }
    public int DuesPeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? RegistryNumber { get; set; }
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Ödeme yöntemini seçmelisiniz.")]
    [Display(Name = "Ödeme Yöntemi")]
    public string PaymentMethod { get; set; } = DuesPaymentMethods.BankTransfer;

    [Required(ErrorMessage = "Ödeme tarihi zorunludur.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Ödeme Tarihi")]
    public DateTime PaidAtLocal { get; set; } = DateTime.Now;

    [StringLength(120)]
    [Display(Name = "Dekont / Referans Numarası")]
    public string? ReferenceNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Yönetici Notu")]
    public string? AdminNote { get; set; }
}
