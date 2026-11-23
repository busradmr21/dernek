using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class ConfidentialReportsAdminViewModel
{
    public IReadOnlyList<ConfidentialReport> Items { get; set; } =
        Array.Empty<ConfidentialReport>();

    public string? Search { get; set; }
    public string? Status { get; set; }
    public int TotalCount { get; set; }
    public int NewCount { get; set; }
}
