using SigortaDernegi.Models;

namespace SigortaDernegi.ViewModels;

public class CorporatePageViewModel
{
	public CorporatePage Page { get; set; } = null!;

	public IReadOnlyList<CorporateContentSection> Sections
	{ get; set; } = Array.Empty<CorporateContentSection>();

	public IReadOnlyList<CorporatePage> NavigationPages
	{ get; set; } = Array.Empty<CorporatePage>();

	public IReadOnlyList<BoardMember> BoardMembers
	{ get; set; } = Array.Empty<BoardMember>();
}