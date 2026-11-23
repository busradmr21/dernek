using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class VolunteerApplicationsAdminViewModel
{
    public IReadOnlyList<VolunteerApplicationAdminItemViewModel> Items { get; set; } = Array.Empty<VolunteerApplicationAdminItemViewModel>();
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int TotalCount { get; set; }
    public int NewCount { get; set; }
}

public class VolunteerApplicationAdminItemViewModel
{
    public Volunteer Application { get; set; } = new();
    public VolunteerApplicationMetadata Metadata { get; set; } = new();
}
