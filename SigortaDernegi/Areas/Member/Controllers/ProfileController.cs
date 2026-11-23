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
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var member = await GetOwnMemberAsync(asTracking: false);
        return member == null ? Forbid() : View(CreateViewModel(member));
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var member = await GetOwnMemberAsync(asTracking: false);
        return member == null ? Forbid() : View(CreateViewModel(member));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MemberProfileViewModel model)
    {
        var member = await GetOwnMemberAsync(asTracking: true);
        if (member == null)
        {
            return Forbid();
        }

        // Kimlik ve üyelik alanları her zaman sunucudaki güvenilir kayıttan gelir.
        ApplyReadOnlyValues(model, member);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        member.Specialty = model.Specialty.Trim();
        member.City = model.City.Trim();
        member.Region = Clean(model.Region);
        member.Phone = Clean(model.Phone);
        member.ShortDescription = Clean(model.ShortDescription);
        member.HasPublicationConsent = model.HasPublicationConsent;

        if (!member.HasPublicationConsent)
        {
            member.IsPublished = false;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Profil bilgileriniz başarıyla güncellendi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<MemberProfile?> GetOwnMemberAsync(bool asTracking)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = _context.MemberProfiles
            .Include(item => item.IdentityUser)
            .Where(item =>
                item.IdentityUserId == identityUserId &&
                item.IsActive);

        if (!asTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync();
    }

    private static MemberProfileViewModel CreateViewModel(
        MemberProfile member)
    {
        var model = new MemberProfileViewModel
        {
            Specialty = member.Specialty,
            City = member.City,
            Region = member.Region,
            Phone = member.Phone,
            ShortDescription = member.ShortDescription,
            HasPublicationConsent = member.HasPublicationConsent
        };

        ApplyReadOnlyValues(model, member);
        return model;
    }

    private static void ApplyReadOnlyValues(
        MemberProfileViewModel model,
        MemberProfile member)
    {
        model.DisplayName = member.DisplayName;
        model.MemberType = member.MemberType;
        model.RegistryNumber = member.RegistryNumber;
        model.LoginEmail = member.IdentityUser?.Email ?? string.Empty;
        model.IsActive = member.IsActive;
        model.IsPublished = member.IsPublished;
        model.MembershipDate =
            member.MembershipApprovedAtUtc ?? member.CreatedAt;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
