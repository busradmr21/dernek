namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberMessagesViewModel
{
    public IReadOnlyList<MemberMessageItemViewModel> Messages { get; set; }
        = Array.Empty<MemberMessageItemViewModel>();
}

public class MemberMessageItemViewModel
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
