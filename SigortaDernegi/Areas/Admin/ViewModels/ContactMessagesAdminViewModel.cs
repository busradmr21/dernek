using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class ContactMessagesAdminViewModel
{
    public IReadOnlyList<ContactMessageAdminItemViewModel> Items { get; set; } = Array.Empty<ContactMessageAdminItemViewModel>();
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int TotalCount { get; set; }
    public int NewCount { get; set; }
    public int MemberMessageCount { get; set; }
}

public class ContactMessageAdminItemViewModel
{
    public ContactMessage Message { get; set; } = new();
    public string? AdminNote { get; set; }
}
