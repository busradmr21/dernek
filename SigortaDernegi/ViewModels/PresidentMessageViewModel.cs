using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public sealed class PresidentMessageViewModel
{
    public BoardMember? President { get; init; }

    public string Title { get; init; } = PresidentMessageDefaults.Title;

    public string Summary { get; init; } = PresidentMessageDefaults.Summary;

    public string Content { get; init; } = PresidentMessageDefaults.Content;

    public string PhotoUrl { get; init; } = PresidentMessageDefaults.PhotoUrl;
}
