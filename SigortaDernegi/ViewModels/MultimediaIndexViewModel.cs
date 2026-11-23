using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public class MultimediaIndexViewModel
{
    public IReadOnlyList<PressArticle> GalleryItems { get; set; } = Array.Empty<PressArticle>();
    public IReadOnlyList<PressArticle> PressItems { get; set; } = Array.Empty<PressArticle>();
}
