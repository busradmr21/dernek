using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public class VolunteerPageViewModel
{
    public Volunteer Application { get; set; } = new();
    public VolunteerSettings Settings { get; set; } = new();
    public bool HasReadKvkkNotice { get; set; }
    public string? Website { get; set; }

    public IReadOnlyList<string> OpportunityAreas => Split(Settings.OpportunityAreas);
    public IReadOnlyList<string> Benefits => Split(Settings.Benefits);
    public IReadOnlyList<string> ProcessSteps => Split(Settings.ProcessSteps);

    private static IReadOnlyList<string> Split(string value) => value
        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();
}
