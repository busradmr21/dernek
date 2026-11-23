using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Areas.Admin.ViewModels;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeedData.AdminRole)]
public class RolePermissionsController : Controller
{
    private readonly ApplicationDbContext _context;

    private static readonly RoleDefinition[] ManagedRoles =
    [
        new(
            IdentitySeedData.AdminRole,
            "Başkan",
            "Tüm sistem yetkilerine sahiptir.",
            "bi-shield-fill-check",
            true),

        new(
            IdentitySeedData.VicePresidentRole,
            "Başkan Yardımcısı",
            "Operasyon ve yönetim süreçlerine destek olur.",
            "bi-person-gear",
            false),

        new(
            IdentitySeedData.TreasurerRole,
            "Sayman",
            "Aidat, ödeme ve finans süreçlerini yönetir.",
            "bi-cash-coin",
            false),

        new(
            IdentitySeedData.GeneralSecretaryRole,
            "Genel Sekreter",
            "Evrak, iletişim ve içerik süreçlerini yönetir.",
            "bi-folder-check",
            false)
    ];

    private static readonly AreaDefinition[] ManagementAreas =
    [
        new(
            ManagementArea.MemberManagement,
            "Üye Yönetimi",
            "Üyeler, uzmanlar ve profil bilgileri",
            "bi-person-vcard"),

        new(
            ManagementArea.DuesAndPayments,
            "Aidat ve Ödemeler",
            "PayTR işlemleri ve finans kayıtları",
            "bi-wallet2"),

        new(
            ManagementArea.DonationManagement,
            "Bağış Yönetimi",
            "Bağış bildirimleri ve bağışçı kayıtları",
            "bi-heart"),

        new(
            ManagementArea.SiteContent,
            "Site İçerikleri",
            "Kurumsal sayfalar, slider ve diğer site içerikleri",
            "bi-megaphone"),

        new(
            ManagementArea.HomeUpdates,
            "Ana Sayfa Etkinlik ve Haberleri",
            "Ana sayfadaki etkinlik ve güncel haber kartları",
            "bi-layout-text-window-reverse"),

        new(
            ManagementArea.ApplicationsAndCommunication,
            "Başvuru ve İletişim",
            "Başvurular, mesajlar ve geri dönüşler",
            "bi-envelope-paper"),

        new(
            ManagementArea.BoardManagement,
            "Dernek Yönetim Üyeleri",
            "Yönetici profilleri, görevler ve dönemler",
            "bi-people"),

        new(
            ManagementArea.ConfidentialReports,
            "Gizli İhbarlar",
            "Gizli ihbar kayıtları, durumları ve ekli belgeler",
            "bi-shield-lock")
    ];

    public RolePermissionsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? role)
    {
        var selectedRole =
            FindRole(role) ?? ManagedRoles[0];

        var roleNames = ManagedRoles
            .Select(item => item.RoleName)
            .ToArray();

        var storedPermissions =
            await _context.RolePermissions
                .AsNoTracking()
                .Where(permission =>
                    roleNames.Contains(permission.RoleName))
                .ToListAsync();

        var model = new RolePermissionsViewModel
        {
            SelectedRoleName = selectedRole.RoleName,
            SelectedRoleDisplayName =
                selectedRole.DisplayName,
            SelectedRoleDescription =
                selectedRole.Description,
            IsLocked = selectedRole.IsLocked,
            Roles = BuildRoleList(storedPermissions),
            Permissions = BuildPermissionList(
                selectedRole,
                storedPermissions)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(
        RolePermissionsViewModel model)
    {
        var selectedRole =
            FindRole(model.SelectedRoleName);

        if (selectedRole == null)
        {
            return BadRequest();
        }

        if (selectedRole.IsLocked)
        {
            TempData["ErrorMessage"] =
                "Başkanın sistem yetkileri değiştirilemez.";

            return RedirectToAction(
                nameof(Index),
                new { role = selectedRole.RoleName });
        }

        var requiredAreas =
            ManagementAreas
                .Select(item => item.Area)
                .ToArray();

        var submittedAreas =
            model.Permissions
                .Select(item => item.Area)
                .ToArray();

        var hasDuplicateArea =
            submittedAreas
                .GroupBy(area => area)
                .Any(group => group.Count() > 1);

        var hasMissingArea =
            requiredAreas.Any(area =>
                !submittedAreas.Contains(area));

        if (hasDuplicateArea || hasMissingArea)
        {
            TempData["ErrorMessage"] =
                "Yetki listesi geçerli değil. Sayfayı yenileyip tekrar deneyin.";

            return RedirectToAction(
                nameof(Index),
                new { role = selectedRole.RoleName });
        }

        var storedPermissions =
            await _context.RolePermissions
                .Where(permission =>
                    permission.RoleName ==
                    selectedRole.RoleName)
                .ToListAsync();

        foreach (var submitted in model.Permissions)
        {
            var permission =
                storedPermissions.SingleOrDefault(item =>
                    item.Area == submitted.Area);

            if (permission == null)
            {
                permission = new RolePermission
                {
                    RoleName = selectedRole.RoleName,
                    Area = submitted.Area
                };

                _context.RolePermissions.Add(permission);
            }

            var hasActionPermission =
                submitted.CanAdd ||
                submitted.CanEdit ||
                submitted.CanApprove ||
                submitted.CanDelete;

            permission.CanView =
                submitted.CanView ||
                hasActionPermission;

            permission.CanAdd = submitted.CanAdd;
            permission.CanEdit = submitted.CanEdit;
            permission.CanApprove = submitted.CanApprove;
            permission.CanDelete = submitted.CanDelete;
            permission.UpdatedAtUtc = DateTime.UtcNow;
            permission.UpdatedBy =
                User.Identity?.Name;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"{selectedRole.DisplayName} yetkileri başarıyla kaydedildi.";

        return RedirectToAction(
            nameof(Index),
            new { role = selectedRole.RoleName });
    }

    private static RoleDefinition? FindRole(
        string? roleName)
    {
        return ManagedRoles.FirstOrDefault(item =>
            string.Equals(
                item.RoleName,
                roleName,
                StringComparison.OrdinalIgnoreCase));
    }

    private static List<RolePermissionRoleViewModel>
        BuildRoleList(
            IReadOnlyCollection<RolePermission>
                storedPermissions)
    {
        const int permissionCountPerArea = 5;

        return ManagedRoles
            .Select(role =>
            {
                var rolePermissions =
                    storedPermissions
                        .Where(permission =>
                            permission.RoleName ==
                            role.RoleName)
                        .ToList();

                var enabledCount = role.IsLocked
                    ? ManagementAreas.Length *
                      permissionCountPerArea
                    : CountEnabled(rolePermissions);

                return new RolePermissionRoleViewModel
                {
                    RoleName = role.RoleName,
                    DisplayName = role.DisplayName,
                    Description = role.Description,
                    IconClass = role.IconClass,
                    EnabledPermissionCount =
                        enabledCount,
                    TotalPermissionCount =
                        ManagementAreas.Length *
                        permissionCountPerArea,
                    IsLocked = role.IsLocked
                };
            })
            .ToList();
    }

    private static List<RolePermissionItemViewModel>
        BuildPermissionList(
            RoleDefinition selectedRole,
            IReadOnlyCollection<RolePermission>
                storedPermissions)
    {
        var selectedPermissions =
            storedPermissions
                .Where(permission =>
                    permission.RoleName ==
                    selectedRole.RoleName)
                .ToDictionary(
                    permission => permission.Area);

        return ManagementAreas
            .Select(area =>
            {
                selectedPermissions.TryGetValue(
                    area.Area,
                    out var stored);

                return new RolePermissionItemViewModel
                {
                    Area = area.Area,
                    AreaName = area.DisplayName,
                    Description = area.Description,
                    IconClass = area.IconClass,
                    CanView =
                        selectedRole.IsLocked ||
                        stored?.CanView == true,
                    CanAdd =
                        selectedRole.IsLocked ||
                        stored?.CanAdd == true,
                    CanEdit =
                        selectedRole.IsLocked ||
                        stored?.CanEdit == true,
                    CanApprove =
                        selectedRole.IsLocked ||
                        stored?.CanApprove == true,
                    CanDelete =
                        selectedRole.IsLocked ||
                        stored?.CanDelete == true
                };
            })
            .ToList();
    }

    private static int CountEnabled(
        IEnumerable<RolePermission> permissions)
    {
        return permissions.Sum(permission =>
            (permission.CanView ? 1 : 0) +
            (permission.CanAdd ? 1 : 0) +
            (permission.CanEdit ? 1 : 0) +
            (permission.CanApprove ? 1 : 0) +
            (permission.CanDelete ? 1 : 0));
    }

    private sealed record RoleDefinition(
        string RoleName,
        string DisplayName,
        string Description,
        string IconClass,
        bool IsLocked);

    private sealed record AreaDefinition(
        ManagementArea Area,
        string DisplayName,
        string Description,
        string IconClass);
}
