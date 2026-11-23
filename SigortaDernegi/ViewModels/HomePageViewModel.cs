using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public class HomePageViewModel
{
    public IReadOnlyList<AssociationEvent> UpcomingEvents { get; init; } =
        Array.Empty<AssociationEvent>();

    public IReadOnlyList<News> LatestNews { get; init; } =
        Array.Empty<News>();

    public HomeContactFormViewModel ContactForm { get; set; } = new();

    public ConfidentialReportFormViewModel ReportForm { get; set; } = new();
}
