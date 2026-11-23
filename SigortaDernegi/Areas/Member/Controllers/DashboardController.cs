using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Member.Authorization;
using SigortaDernegi.Areas.Member.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Policy = MemberPortalPolicies.ActiveMember)]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var member = await _context.MemberProfiles
            .AsNoTracking()
            .Include(item => item.IdentityUser)
            .SingleOrDefaultAsync(item =>
                item.IdentityUserId == identityUserId &&
                item.IsActive);

        if (member == null)
        {
            return Forbid();
        }

        var announcements = await _context.News
            .AsNoTracking()
            .Where(item => item.IsActive && item.IsAnnouncement)
            .OrderByDescending(item => item.CreatedAt)
            .Take(3)
            .Select(item => new MemberAnnouncementItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Summary = item.Summary,
                PublishedAt = item.CreatedAt
            })
            .ToListAsync();

        var now = DateTime.Now;
        var upcomingEvents = await _context.AssociationEvents
            .AsNoTracking()
            .Where(item => item.IsActive && item.StartDate >= now)
            .OrderBy(item => item.StartDate)
            .Take(3)
            .Select(item => new MemberEventItemViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Summary = item.Summary,
                StartDate = item.StartDate,
                Location = item.Location,
                IsOnline = item.IsOnline
            })
            .ToListAsync();

        var messageCount = await _context.ContactMessages
            .AsNoTracking()
            .CountAsync(message =>
                message.RecipientMemberProfileId == member.Id);

        var outstandingDues = await _context.MemberDues
            .AsNoTracking()
            .Where(due =>
                due.MemberProfileId == member.Id &&
                due.Status == MemberDueStatuses.Unpaid)
            .Select(due => due.Amount)
            .ToListAsync();

        return View(new MemberDashboardViewModel
        {
            DisplayName = member.DisplayName,
            MemberType = member.MemberType,
            RegistryNumber = member.RegistryNumber,
            Specialty = member.Specialty,
            City = member.City,
            LoginEmail = member.IdentityUser?.Email ?? User.Identity?.Name ?? string.Empty,
            MembershipDate = member.MembershipApprovedAtUtc ?? member.CreatedAt,
            IsPublished = member.IsPublished,
            MessageCount = messageCount,
            OutstandingDuesCount = outstandingDues.Count,
            OutstandingDuesAmount = outstandingDues.Sum(),
            Announcements = announcements,
            UpcomingEvents = upcomingEvents
        });
    }
}
