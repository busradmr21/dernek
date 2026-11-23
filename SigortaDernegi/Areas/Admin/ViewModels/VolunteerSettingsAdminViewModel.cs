using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class VolunteerSettingsAdminViewModel
{
    public VolunteerSettings Settings { get; set; } = new();
    public IReadOnlyList<ContactSettingsRevisionViewModel> Revisions { get; set; } = Array.Empty<ContactSettingsRevisionViewModel>();
}
