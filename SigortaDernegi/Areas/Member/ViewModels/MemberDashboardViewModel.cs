namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberDashboardViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string MemberType { get; set; } = string.Empty;
    public string? RegistryNumber { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string LoginEmail { get; set; } = string.Empty;
    public DateTime MembershipDate { get; set; }
    public bool IsPublished { get; set; }
    public int MessageCount { get; set; }
    public int OutstandingDuesCount { get; set; }
    public decimal OutstandingDuesAmount { get; set; }

    public IReadOnlyList<MemberAnnouncementItemViewModel> Announcements
        { get; set; } = Array.Empty<MemberAnnouncementItemViewModel>();

    public IReadOnlyList<MemberEventItemViewModel> UpcomingEvents
        { get; set; } = Array.Empty<MemberEventItemViewModel>();
}

public class MemberAnnouncementItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
}

public class MemberEventItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? Location { get; set; }
    public bool IsOnline { get; set; }
}
