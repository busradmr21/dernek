using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Models;

namespace SigortaDernegi.Data;

public static class RolePermissionSeedData
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        var permissions = new List<RolePermission>();

        // Dernek Başkanı (Admin): bütün yetkiler açık
        foreach (var area in Enum.GetValues<ManagementArea>())
        {
            permissions.Add(Create(
                IdentitySeedData.AdminRole,
                area,
                view: true,
                add: true,
                edit: true,
                approve: true,
                delete: true));
        }

        // Başkan Yardımcısı
        permissions.AddRange(
        [
            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.MemberManagement,
                true, true, true, true, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.DuesAndPayments,
                true, false, false, false, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.DonationManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.SiteContent,
                true, true, true, true, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.ApplicationsAndCommunication,
                true, true, true, true, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.BoardManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.HomeUpdates,
                true, true, true, true, false),

            Create(
                IdentitySeedData.VicePresidentRole,
                ManagementArea.ConfidentialReports,
                false, false, false, false, false)
        ]);

        // Sayman
        permissions.AddRange(
        [
            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.MemberManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.DuesAndPayments,
                true, true, true, true, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.DonationManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.SiteContent,
                true, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.ApplicationsAndCommunication,
                false, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.BoardManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.HomeUpdates,
                true, false, false, false, false),

            Create(
                IdentitySeedData.TreasurerRole,
                ManagementArea.ConfidentialReports,
                false, false, false, false, false)
        ]);

        // Genel Sekreter
        permissions.AddRange(
        [
            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.MemberManagement,
                true, true, true, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.DuesAndPayments,
                true, false, false, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.DonationManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.SiteContent,
                true, true, true, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.ApplicationsAndCommunication,
                true, true, true, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.BoardManagement,
                true, false, false, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.HomeUpdates,
                true, true, true, false, false),

            Create(
                IdentitySeedData.GeneralSecretaryRole,
                ManagementArea.ConfidentialReports,
                false, false, false, false, false)
        ]);

        const int removedManagementAreaValue = 3;
        var obsoletePermissions =
            await context.RolePermissions
                .Where(permission =>
                    (int)permission.Area ==
                    removedManagementAreaValue)
                .ToListAsync();

        if (obsoletePermissions.Count > 0)
        {
            context.RolePermissions.RemoveRange(
                obsoletePermissions);
        }

        var existingPermissions =
            await context.RolePermissions
                .AsNoTracking()
                .Where(permission =>
                    (int)permission.Area !=
                    removedManagementAreaValue)
                .Select(permission => new
                {
                    permission.RoleName,
                    permission.Area
                })
                .ToListAsync();

        foreach (var permission in permissions)
        {
            var alreadyExists =
                existingPermissions.Any(existing =>
                    existing.RoleName == permission.RoleName &&
                    existing.Area == permission.Area);

            if (!alreadyExists)
            {
                context.RolePermissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();
    }

    private static RolePermission Create(
        string roleName,
        ManagementArea area,
        bool view,
        bool add,
        bool edit,
        bool approve,
        bool delete)
    {
        return new RolePermission
        {
            RoleName = roleName,
            Area = area,
            CanView = view,
            CanAdd = add,
            CanEdit = edit,
            CanApprove = approve,
            CanDelete = delete,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
