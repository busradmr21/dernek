using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public class ContactPageViewModel
{
    public ContactMessage Message { get; set; } = new();
    public ContactSettings Settings { get; set; } = new();
    public string? Website { get; set; }

    public IReadOnlyList<string> Subjects => Settings.SubjectOptions
        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
}
