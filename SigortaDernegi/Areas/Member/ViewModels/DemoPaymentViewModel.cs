namespace SigortaDernegi.Areas.Member.ViewModels;

public class DemoPaymentViewModel
{
    public int MemberDueId { get; set; }  //Ödenecek aidat kaydı

    public string MemberName { get; set; } = string.Empty; //Ödemeyi yapan üyenin adı

    public string PeriodName { get; set; } = string.Empty;  //Aidatın hangi döneme ait olduğu

    public decimal Amount { get; set; }  //Yönetimin belirlediği sabit ödeme tutarı

    public DateTime DueDate { get; set; }  //Son ödeme tarihi

    public bool IsOverdue { get; set; }  //Aidatın gecikip gecikmediği
}