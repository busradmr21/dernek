using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class ContactSettingsAdminViewModel
{
    public ContactSettings Settings { get; set; } = new();
    public IReadOnlyList<ContactSettingsRevisionViewModel> Revisions { get; set; } = Array.Empty<ContactSettingsRevisionViewModel>();
}

public class ContactSettingsRevisionViewModel
{
    public int Id { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}
