using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
public class MemberManagementController : Controller
{
    private static readonly string[] AllowedMemberTypes =
        ["Bireysel", "Kurumsal"];

    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MemberManagementController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.View)]
    public async Task<IActionResult> Index(
        string? search,
        string? memberType,
        string? status)
    {
        var query = _context.MemberProfiles.AsNoTracking();

        search = Clean(search);
        memberType = Clean(memberType);
        status = Clean(status);

        if (search != null)
        {
            query = query.Where(member =>
                member.DisplayName.Contains(search) ||
                member.Specialty.Contains(search) ||
                member.City.Contains(search) ||
                (member.RegistryNumber != null &&
                 member.RegistryNumber.Contains(search)));
        }

        if (AllowedMemberTypes.Contains(memberType))
        {
            query = query.Where(member =>
                member.MemberType == memberType);
        }

        query = status switch
        {
            "active" => query.Where(member => member.IsActive),
            "passive" => query.Where(member => !member.IsActive),
            "published" => query.Where(member => member.IsPublished),
            "hidden" => query.Where(member => !member.IsPublished),
            _ => query
        };

        ViewBag.Search = search;
        ViewBag.MemberType = memberType;
        ViewBag.Status = status;
        ViewBag.TotalCount = await _context.MemberProfiles.CountAsync();
        ViewBag.ActiveCount = await _context.MemberProfiles
            .CountAsync(member => member.IsActive);
        ViewBag.PublishedCount = await _context.MemberProfiles
            .CountAsync(member =>
                member.IsActive &&
                member.HasPublicationConsent &&
                member.IsPublished);

        var members = await query
            .OrderByDescending(member => member.IsActive)
            .ThenBy(member => member.DisplayName)
            .ToListAsync();

        return View(members);
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Add)]
    public IActionResult Create()
    {
        return View(new MemberProfileFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Add)]
    public async Task<IActionResult> Create(
        MemberProfileFormViewModel model)
    {
        // Yayın açık rızası yönetici tarafından üye adına verilemez.
        // Yeni üye, bu seçimi ilk girişinden sonra kendi panelinden yapar.
        model.HasPublicationConsent = false;
        model.IsPublished = false;
        await ValidateFormAsync(model);
        await ValidateNewMemberAccountAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var loginEmail = model.LoginEmail!.Trim();
        var identityUser = new IdentityUser
        {
            UserName = loginEmail,
            Email = loginEmail,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            identityUser,
            model.TemporaryPassword!);

        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return View(model);
        }

        var roleResult = await _userManager.AddToRoleAsync(
            identityUser,
            IdentitySeedData.MemberRole);

        if (!roleResult.Succeeded)
        {
            AddIdentityErrors(roleResult);
            return View(model);
        }

        var member = new MemberProfile
        {
            MemberType = model.MemberType,
            DisplayName = model.DisplayName.Trim(),
            RegistryNumber = Clean(model.RegistryNumber),
            Specialty = model.Specialty.Trim(),
            City = model.City.Trim(),
            Region = Clean(model.Region),
            ShortDescription = Clean(model.ShortDescription),
            IdentityUserId = identityUser.Id,
            IsActive = model.IsActive,
            HasPublicationConsent = false,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.MemberProfiles.Add(member);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["SuccessMessage"] =
            "Normal üye kaydı ve giriş hesabı başarıyla oluşturuldu. Geçici şifreyi üyeye güvenli bir kanaldan iletin.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var member = await _context.MemberProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        return View(new MemberProfileFormViewModel
        {
            Id = member.Id,
            MemberType = member.MemberType,
            DisplayName = member.DisplayName,
            RegistryNumber = member.RegistryNumber,
            Specialty = member.Specialty,
            City = member.City,
            Region = member.Region,
            ShortDescription = member.ShortDescription,
            IsActive = member.IsActive,
            HasPublicationConsent = member.HasPublicationConsent,
            IsPublished = member.IsPublished
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(
        int id,
        MemberProfileFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var member = await _context.MemberProfiles
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        // İstek içeriğinden gelen değer kullanılmaz; açık rıza yalnızca üyenin
        // kendi panelindeki işleminden alınır ve burada salt okunur tutulur.
        model.HasPublicationConsent = member.HasPublicationConsent;
        if (!member.HasPublicationConsent)
        {
            model.IsPublished = false;
        }

        await ValidateFormAsync(model, id);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        member.MemberType = model.MemberType;
        member.DisplayName = model.DisplayName.Trim();
        member.RegistryNumber = Clean(model.RegistryNumber);
        member.Specialty = model.Specialty.Trim();
        member.City = model.City.Trim();
        member.Region = Clean(model.Region);
        member.ShortDescription = Clean(model.ShortDescription);
        member.IsActive = model.IsActive;
        member.IsPublished = model.IsPublished;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Üye bilgileri başarıyla güncellendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Edit)]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var member = await _context.MemberProfiles
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        member.IsActive = !member.IsActive;

        if (!member.IsActive)
        {
            member.IsPublished = false;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = member.IsActive
            ? "Üye yeniden faal duruma getirildi."
            : "Üye pasif duruma getirildi ve rehberden kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Approve)]
    public async Task<IActionResult> TogglePublication(int id)
    {
        var member = await _context.MemberProfiles
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        if (!member.IsPublished &&
            (!member.IsActive ||
             !member.HasPublicationConsent))
        {
            TempData["ErrorMessage"] =
                "Bir üyeyi rehberde yayımlamak için üyenin faal olması ve " +
                "yayın izninin bulunması gerekir.";

            return RedirectToAction(nameof(Index));
        }

        member.IsPublished = !member.IsPublished;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = member.IsPublished
            ? "Üye, Uzman ve Üye Rehberinde yayımlandı."
            : "Üye, Uzman ve Üye Rehberinden kaldırıldı.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.MemberManagement,
        PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _context.MemberProfiles
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        if (member.IdentityUserId != null)
        {
            TempData["ErrorMessage"] =
                "Giriş hesabı bulunan bir üye kalıcı olarak silinemez. Üyeliği sonlandırmak için üyeyi pasif duruma getirin.";

            return RedirectToAction(nameof(Index));
        }

        _context.MemberProfiles.Remove(member);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Üye kaydı kalıcı olarak silindi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateFormAsync(
        MemberProfileFormViewModel model,
        int? currentMemberId = null)
    {
        if (!AllowedMemberTypes.Contains(model.MemberType))
        {
            ModelState.AddModelError(
                nameof(model.MemberType),
                "Geçerli bir üye türü seçmelisiniz.");
        }

        if (model.IsPublished && !model.IsActive)
        {
            ModelState.AddModelError(
                nameof(model.IsPublished),
                "Pasif bir üye rehberde yayımlanamaz.");
        }

        if (model.IsPublished && !model.HasPublicationConsent)
        {
            ModelState.AddModelError(
                nameof(model.IsPublished),
                "Üyenin yayın izni olmadan rehberde yayımlanamaz.");
        }

        var registryNumber = Clean(model.RegistryNumber);

        if (registryNumber == null)
        {
            return;
        }

        var duplicateQuery = _context.MemberProfiles
            .AsNoTracking()
            .Where(member => member.RegistryNumber == registryNumber);

        if (currentMemberId.HasValue)
        {
            duplicateQuery = duplicateQuery.Where(
                member => member.Id != currentMemberId.Value);
        }

        if (await duplicateQuery.AnyAsync())
        {
            ModelState.AddModelError(
                nameof(model.RegistryNumber),
                "Bu üye numarası başka bir kayıtta kullanılıyor.");
        }
    }

    private async Task ValidateNewMemberAccountAsync(
        MemberProfileFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.LoginEmail))
        {
            ModelState.AddModelError(
                nameof(model.LoginEmail),
                "Giriş e-posta adresi zorunludur.");
        }
        else
        {
            var existingUser = await _userManager.FindByEmailAsync(
                model.LoginEmail.Trim());

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.LoginEmail),
                    "Bu e-posta adresiyle kayıtlı bir giriş hesabı zaten var.");
            }
        }

        if (string.IsNullOrWhiteSpace(model.TemporaryPassword))
        {
            ModelState.AddModelError(
                nameof(model.TemporaryPassword),
                "Geçici şifre zorunludur.");
        }

        if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
        {
            ModelState.AddModelError(
                nameof(model.ConfirmPassword),
                "Geçici şifre tekrarı zorunludur.");
        }
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
