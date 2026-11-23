using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;

namespace SigortaDernegi.Areas.Member.Authorization;

public static class MemberPortalPolicies
{
    public const string ActiveMember = "ActiveMember";
}

public sealed class ActiveMemberRequirement : IAuthorizationRequirement;

public sealed class ActiveMemberHandler
    : AuthorizationHandler<ActiveMemberRequirement>
{
    private readonly ApplicationDbContext _context;

    public ActiveMemberHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveMemberRequirement requirement)
    {
        var identityUserId = context.User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return;
        }

        var hasActiveProfile = await _context.MemberProfiles
            .AsNoTracking()
            .AnyAsync(member =>
                member.IdentityUserId == identityUserId &&
                member.IsActive);

        if (hasActiveProfile)
        {
            context.Succeed(requirement);
        }
    }
}
