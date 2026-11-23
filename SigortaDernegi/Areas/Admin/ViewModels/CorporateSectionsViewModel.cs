using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class CorporateSectionsViewModel
{
    public CorporatePage Page { get; set; } = null!;

    public IReadOnlyList<CorporateContentSection> Sections { get; set; } =
        Array.Empty<CorporateContentSection>();
}
