using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SigortaDernegi.Areas.Member.Authorization;
using SigortaDernegi.Areas.Member.ViewModels;

namespace SigortaDernegi.Areas.Member.Controllers;

[Area("Member")]
[Authorize(Policy = MemberPortalPolicies.ActiveMember)]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Security()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        return View(new MemberSecurityViewModel
        {
            Email = user.Email ?? user.UserName ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            HasPassword = await _userManager.HasPasswordAsync(user)
        });
    }
}
