using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;
using SigortaDernegi.ViewModels;

namespace SigortaDernegi.Controllers;

public class AboutController : Controller
{
	private readonly ApplicationDbContext _context;

	public AboutController(ApplicationDbContext context)
	{
		_context = context;
	}

	public Task<IActionResult> Index()
	{
		return ShowPageAsync("about", nameof(Index));
	}

	public Task<IActionResult> Bylaws()
	{
		return ShowPageAsync("bylaws", nameof(Bylaws));
	}

	public Task<IActionResult> Legislation()
	{
		return ShowPageAsync(
			"legislation",
			nameof(Legislation));
	}

	public Task<IActionResult> Board()
	{
		return ShowPageAsync("board", nameof(Board));
	}

	public Task<IActionResult> Committees()
	{
		return ShowPageAsync(
			"committees",
			nameof(Committees));
	}

	[HttpGet]
	public async Task<IActionResult> PresidentMessage()
	{
		var president = await _context.BoardMembers
			.AsNoTracking()
			.Where(member =>
				member.Position == BoardPosition.President &&
				member.IsActive &&
				member.IsPublished)
			.OrderBy(member => member.DisplayOrder)
			.ThenBy(member => member.Id)
			.FirstOrDefaultAsync();

		var model = new PresidentMessageViewModel
		{
			President = president,
			Title = string.IsNullOrWhiteSpace(
				president?.HomepageMessageTitle)
				? PresidentMessageDefaults.Title
				: president.HomepageMessageTitle,
			Summary = string.IsNullOrWhiteSpace(
				president?.HomepageMessageSummary)
				? PresidentMessageDefaults.Summary
				: president.HomepageMessageSummary,
			Content = string.IsNullOrWhiteSpace(
				president?.PresidentMessageContent)
				? PresidentMessageDefaults.Content
				: president.PresidentMessageContent,
			PhotoUrl = string.IsNullOrWhiteSpace(president?.PhotoUrl)
				? PresidentMessageDefaults.PhotoUrl
				: president.PhotoUrl
		};

		return View(model);
	}

	private async Task<IActionResult> ShowPageAsync(
		string pageKey,
		string viewName)
	{
		var page = await _context.CorporatePages
			.AsNoTracking()
			.SingleOrDefaultAsync(item =>
				item.PageKey == pageKey &&
				item.IsActive);

		if (page == null)
		{
			return NotFound();
		}

		var sections = await _context
			.CorporateContentSections
			.AsNoTracking()
			.Where(section =>
				section.CorporatePageId == page.Id &&
				section.IsActive)
			.OrderBy(section => section.DisplayOrder)
			.ThenBy(section => section.Id)
			.ToListAsync();

		var navigationPages = await _context
			.CorporatePages
			.AsNoTracking()
			.Where(item => item.IsActive)
			.OrderBy(item => item.DisplayOrder)
			.ThenBy(item => item.Id)
			.ToListAsync();

		IReadOnlyList<BoardMember> boardMembers =
			Array.Empty<BoardMember>();

		if (pageKey == "board")
		{
			boardMembers = await _context.BoardMembers
				.AsNoTracking()
				.Where(member =>
					member.IsActive &&
					member.IsPublished)
				.OrderBy(member => member.DisplayOrder)
				.ThenBy(member => member.Position)
				.ThenBy(member => member.FullName)
				.ToListAsync();

		}

		var viewModel = new CorporatePageViewModel
		{
			Page = page,
			Sections = sections,
			NavigationPages = navigationPages,
			BoardMembers = boardMembers
		};

		return View(viewName, viewModel);
	}
}
