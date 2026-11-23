namespace SigortaDernegi.Areas.Member.ViewModels;

public class MemberSecurityViewModel
{
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool HasPassword { get; set; }
}
