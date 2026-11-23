namespace SigortaDernegi.Areas.Admin.ViewModels;

public class BoardMemberDetailsViewModel
{
    public BoardMemberFormViewModel Form { get; set; } = new();

    public BoardMemberAccountViewModel Account { get; set; } = new();

    public string ActiveTab { get; set; } = "info";
}
