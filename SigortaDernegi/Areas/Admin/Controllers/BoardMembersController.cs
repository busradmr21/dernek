using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.Authorization;
using SigortaDernegi.Areas.Admin.Services;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.ManagementRoles)]
public class BoardMembersController : Controller
{
    private const string UploadFolder = "board-members";

    private readonly ApplicationDbContext _context;
    private readonly AdminImageStorage _imageStorage;
    private readonly UserManager<IdentityUser> _userManager;

    public BoardMembersController(
        ApplicationDbContext context,
        AdminImageStorage imageStorage,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _imageStorage = imageStorage;
        _userManager = userManager;
    }

    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.View)]
    public async Task<IActionResult> Index()
    {
        var members = await _context.BoardMembers
            .AsNoTracking()
            .Include(member => member.IdentityUser)
            .OrderBy(member => member.DisplayOrder)
            .ThenBy(member => member.Position)
            .ThenBy(member => member.FullName)
            .ToListAsync();

        return View(members);
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Add)]
    public IActionResult Create()
    {
        var model = new BoardMemberFormViewModel
        {
            TermStart = DateTime.Today,
            IsActive = true,
            IsPublished = true,
            HomepageMessageTitle = PresidentMessageDefaults.Title,
            HomepageMessageSummary = PresidentMessageDefaults.Summary,
            PresidentMessageContent = PresidentMessageDefaults.Content
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Add)]
    public async Task<IActionResult> Create(
        BoardMemberFormViewModel model)
    {
        await ValidateFormAsync(model);

        var imageError = _imageStorage.Validate(
            model.PhotoFile,
            required: false);

        if (imageError != null)
        {
            ModelState.AddModelError(
                nameof(model.PhotoFile),
                imageError);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? photoUrl = null;

        if (model.PhotoFile is { Length: > 0 })
        {
            photoUrl = await _imageStorage.SaveAsync(
                model.PhotoFile,
                UploadFolder);
        }

        var member = new BoardMember
        {
            FullName = model.FullName.Trim(),
            Position = model.Position,
            ProfessionalTitle = Clean(model.ProfessionalTitle),
            Biography = Clean(model.Biography),
            HomepageMessageTitle = Clean(model.HomepageMessageTitle),
            HomepageMessageSummary = Clean(model.HomepageMessageSummary),
            PresidentMessageContent = Clean(model.PresidentMessageContent),
            PhotoUrl = photoUrl,
            TermStart = model.TermStart,
            TermEnd = model.TermEnd,
            IsActive = model.IsActive,
            IsPublished = model.IsActive && model.IsPublished,
            DisplayOrder = model.DisplayOrder
        };

        _context.BoardMembers.Add(member);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Dernek yönetim üyesi başarıyla eklendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.View)]
    public async Task<IActionResult> Details(int id, string tab = "info")
    {
        var member = await _context.BoardMembers
            .AsNoTracking()
            .Include(item => item.IdentityUser)
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        var model = await BuildDetailsViewModelAsync(member, activeTab: tab);
        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = IdentitySeedData.AdminRole)]
    public IActionResult Account(int id)
    {
        return RedirectToAction(nameof(Details), new { id, tab = "account" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeedData.AdminRole)]
    public async Task<IActionResult> LinkCurrentAdmin(int id)
    {
        var member = await _context.BoardMembers
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        if (member.Position != BoardPosition.President)
        {
            return BadRequest();
        }

        var currentAdmin = await _userManager.GetUserAsync(User);

        if (currentAdmin == null)
        {
            return Challenge();
        }

        var linkedToAnotherMember = await _context.BoardMembers
            .AsNoTracking()
            .AnyAsync(item =>
                item.Id != member.Id &&
                item.IdentityUserId == currentAdmin.Id);

        if (linkedToAnotherMember)
        {
            TempData["ErrorMessage"] =
                "Bu yönetici hesabı başka bir yönetim üyesine bağlı.";

            return RedirectToAction(nameof(Details), new { id, tab = "account" });
        }

        member.IdentityUserId = currentAdmin.Id;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Başkan kaydı mevcut admin hesabına bağlandı.";

        return RedirectToAction(nameof(Details), new { id, tab = "account" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeedData.AdminRole)]
    public async Task<IActionResult> CreateAccount(
        BoardMemberAccountViewModel model)
    {
        var member = await _context.BoardMembers
            .Include(item => item.IdentityUser)
            .SingleOrDefaultAsync(item => item.Id == model.BoardMemberId);

        if (member == null)
        {
            return NotFound();
        }

        if (member.Position == BoardPosition.President)
        {
            return BadRequest();
        }

        if (member.IdentityUserId != null)
        {
            TempData["ErrorMessage"] =
                "Bu yönetim üyesinin zaten bir giriş hesabı bulunuyor.";

            return RedirectToAction(
                nameof(Details),
                new { id = member.Id, tab = "account" });
        }

        if (!ModelState.IsValid)
        {
            PopulateAccountModel(model, member);
            var detailsModel = await BuildDetailsViewModelAsync(member, account: model, activeTab: "account");
            return View("Details", detailsModel);
        }

        var email = model.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "Bu e-posta adresiyle kayıtlı bir kullanıcı zaten var.");

            PopulateAccountModel(model, member);
            var detailsModel = await BuildDetailsViewModelAsync(member, account: model, activeTab: "account");
            return View("Details", detailsModel);
        }

        var identityUser = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            identityUser,
            model.TemporaryPassword);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            PopulateAccountModel(model, member);
            var detailsModel = await BuildDetailsViewModelAsync(member, account: model, activeTab: "account");
            return View("Details", detailsModel);
        }

        var roleName = GetRoleName(member.Position);
        var roleResult = await _userManager.AddToRoleAsync(
            identityUser,
            roleName);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(identityUser);

            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            PopulateAccountModel(model, member);
            var detailsModel = await BuildDetailsViewModelAsync(member, account: model, activeTab: "account");
            return View("Details", detailsModel);
        }

        try
        {
            member.IdentityUserId = identityUser.Id;
            await _context.SaveChangesAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(identityUser);
            throw;
        }

        TempData["SuccessMessage"] =
            $"{member.FullName} için giriş hesabı oluşturuldu.";

        return RedirectToAction(nameof(Details), new { id = member.Id, tab = "account" });
    }

    [HttpGet]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Edit)]
    public IActionResult Edit(int id)
    {
        return RedirectToAction(nameof(Details), new { id, tab = "info" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Edit)]
    public async Task<IActionResult> Edit(
        int id,
        BoardMemberFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var member = await _context.BoardMembers
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        await ValidateFormAsync(model, id);

        var imageError = _imageStorage.Validate(
            model.PhotoFile,
            required: false);

        if (imageError != null)
        {
            ModelState.AddModelError(
                nameof(model.PhotoFile),
                imageError);
        }

        if (!ModelState.IsValid)
        {
            model.ExistingPhotoUrl = member.PhotoUrl;
            var detailsModel = await BuildDetailsViewModelAsync(member, form: model, activeTab: "info");
            return View("Details", detailsModel);
        }

        string? newPhotoUrl = null;
        var oldPhotoUrl = member.PhotoUrl;

        if (model.PhotoFile is { Length: > 0 })
        {
            newPhotoUrl = await _imageStorage.SaveAsync(
                model.PhotoFile,
                UploadFolder);

            member.PhotoUrl = newPhotoUrl;
        }

        member.FullName = model.FullName.Trim();
        member.Position = model.Position;
        member.ProfessionalTitle =
            Clean(model.ProfessionalTitle);
        member.Biography = Clean(model.Biography);
        member.HomepageMessageTitle =
            Clean(model.HomepageMessageTitle);
        member.HomepageMessageSummary =
            Clean(model.HomepageMessageSummary);
        member.PresidentMessageContent =
            Clean(model.PresidentMessageContent);
        member.TermStart = model.TermStart;
        member.TermEnd = model.TermEnd;
        member.IsActive = model.IsActive;
        member.IsPublished = model.IsActive && model.IsPublished;
        member.DisplayOrder = model.DisplayOrder;

        await _context.SaveChangesAsync();

        if (newPhotoUrl != null)
        {
            _imageStorage.DeleteIfManaged(
                oldPhotoUrl,
                UploadFolder);
        }

        TempData["SuccessMessage"] =
            "Yönetim kurulu üyesi bilgileri başarıyla güncellendi.";

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Approve)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var member = await _context.BoardMembers
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        member.IsPublished = !member.IsPublished;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = member.IsPublished
            ? "Dernek yönetim üyesi sitede gösterilecek."
            : "Dernek yönetim üyesi siteden gizlendi.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(
        ManagementArea.BoardManagement,
        PermissionActionType.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _context.BoardMembers
            .SingleOrDefaultAsync(item => item.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        if (member.IdentityUserId != null)
        {
            TempData["ErrorMessage"] =
                "Giriş hesabı bulunan yönetim üyesi silinemez.";

            return RedirectToAction(nameof(Index));
        }

        var photoUrl = member.PhotoUrl;

        _context.BoardMembers.Remove(member);
        await _context.SaveChangesAsync();

        _imageStorage.DeleteIfManaged(
            photoUrl,
            UploadFolder);

        TempData["SuccessMessage"] =
            "Dernek yönetim üyesi silindi.";

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateFormAsync(
        BoardMemberFormViewModel model,
        int? currentMemberId = null)
    {
        if (!Enum.IsDefined(
                typeof(BoardPosition),
                model.Position))
        {
            ModelState.AddModelError(
                nameof(model.Position),
                "Geçerli bir yönetim görevi seçmelisiniz.");
        }

        if (model.TermEnd.HasValue &&
            model.TermEnd.Value.Date < model.TermStart.Date)
        {
            ModelState.AddModelError(
                nameof(model.TermEnd),
                "Bitiş tarihi başlangıç tarihinden önce olamaz.");
        }

        if (!model.IsActive)
        {
            return;
        }

        var query = _context.BoardMembers
            .AsNoTracking()
            .Where(member =>
                member.Position == model.Position &&
                member.IsActive);

        if (currentMemberId.HasValue)
        {
            query = query.Where(
                member => member.Id != currentMemberId.Value);
        }

        if (await query.AnyAsync())
        {
            ModelState.AddModelError(
                nameof(model.Position),
                "Bu görevde hâlen aktif olan başka bir kişi bulunuyor.");
        }
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static void PopulateAccountModel(
        BoardMemberAccountViewModel model,
        BoardMember member)
    {
        model.BoardMemberId = member.Id;
        model.FullName = member.FullName;
        model.Position = member.Position;
        model.RoleDisplayName = GetRoleDisplayName(member.Position);
        model.HasAccount = member.IdentityUserId != null;
        model.IsPresident = member.Position == BoardPosition.President;
        model.ExistingEmail = member.IdentityUser?.Email;
    }

    private static string GetRoleName(BoardPosition position)
    {
        return position switch
        {
            BoardPosition.President => IdentitySeedData.AdminRole,
            BoardPosition.VicePresident => IdentitySeedData.VicePresidentRole,
            BoardPosition.Treasurer => IdentitySeedData.TreasurerRole,
            BoardPosition.GeneralSecretary => IdentitySeedData.GeneralSecretaryRole,
            _ => throw new ArgumentOutOfRangeException(nameof(position))
        };
    }

    private static string GetRoleDisplayName(BoardPosition position)
    {
        return position switch
        {
            BoardPosition.President => "Başkan / Admin",
            BoardPosition.VicePresident => "Başkan Yardımcısı",
            BoardPosition.Treasurer => "Sayman",
            BoardPosition.GeneralSecretary => "Genel Sekreter",
            _ => "Yönetim Üyesi"
        };
    }

    private async Task<BoardMemberDetailsViewModel> BuildDetailsViewModelAsync(
        BoardMember member,
        BoardMemberFormViewModel? form = null,
        BoardMemberAccountViewModel? account = null,
        string activeTab = "info")
    {
        var currentAdmin = await _userManager.GetUserAsync(User);

        var formModel = form ?? new BoardMemberFormViewModel
        {
            Id = member.Id,
            FullName = member.FullName,
            Position = member.Position,
            ProfessionalTitle = member.ProfessionalTitle,
            Biography = member.Biography,
            HomepageMessageTitle = member.HomepageMessageTitle,
            HomepageMessageSummary = member.HomepageMessageSummary,
            PresidentMessageContent = member.PresidentMessageContent,
            ExistingPhotoUrl = member.PhotoUrl,
            TermStart = member.TermStart,
            TermEnd = member.TermEnd,
            IsActive = member.IsActive,
            IsPublished = member.IsPublished,
            DisplayOrder = member.DisplayOrder
        };

        var accountModel = account ?? new BoardMemberAccountViewModel
        {
            BoardMemberId = member.Id,
            FullName = member.FullName,
            Position = member.Position,
            RoleDisplayName = GetRoleDisplayName(member.Position),
            HasAccount = member.IdentityUserId != null,
            IsPresident = member.Position == BoardPosition.President,
            ExistingEmail = member.IdentityUser?.Email,
            CurrentAdminEmail = currentAdmin?.Email
        };

        if (string.IsNullOrEmpty(accountModel.CurrentAdminEmail))
        {
            accountModel.CurrentAdminEmail = currentAdmin?.Email;
        }

        return new BoardMemberDetailsViewModel
        {
            Form = formModel,
            Account = accountModel,
            ActiveTab = activeTab
        };
    }
}
