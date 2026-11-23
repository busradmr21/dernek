using SigortaDernegi.Areas.Member.ViewModels;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class ManagementDuesViewModel
{
    public bool HasLinkedBoardMembership { get; set; }
    public string? BoardMemberName { get; set; }
    public MemberDuesViewModel Dues { get; set; } = new();
}
