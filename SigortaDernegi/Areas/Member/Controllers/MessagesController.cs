using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Member.Authorization;
using SigortaDernegi.Areas.Member.ViewModels;
using SigortaDernegi.Data;

namespace SigortaDernegi.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Policy = MemberPortalPolicies.ActiveMember)]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MessagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var memberId = await _context.MemberProfiles
            .AsNoTracking()
            .Where(member =>
                member.IdentityUserId == identityUserId &&
                member.IsActive)
            .Select(member => (int?)member.Id)
            .SingleOrDefaultAsync();

        if (!memberId.HasValue)
        {
            return Forbid();
        }

        var messages = await _context.ContactMessages
            .AsNoTracking()
            .Where(message =>
                message.RecipientMemberProfileId == memberId.Value)
            .OrderByDescending(message => message.CreatedAt)
            .Select(message => new MemberMessageItemViewModel
            {
                Id = message.Id,
                SenderName = message.FullName,
                SenderEmail = message.Email,
                SenderPhone = message.Phone,
                Subject = message.Subject,
                Message = message.Message,
                CreatedAt = message.CreatedAt
            })
            .ToListAsync();

        return View(new MemberMessagesViewModel
        {
            Messages = messages
        });
    }
}
