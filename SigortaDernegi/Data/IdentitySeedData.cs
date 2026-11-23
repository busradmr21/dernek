using Microsoft.AspNetCore.Identity;

namespace SigortaDernegi.Data
{
    public static class IdentitySeedData
    {
        public const string AdminRole = "Admin";
        public const string VicePresidentRole = "VicePresident";
        public const string TreasurerRole = "Treasurer";
        public const string GeneralSecretaryRole = "GeneralSecretary";
        public const string MemberRole = "Member";

        public const string ManagementRoles =
            AdminRole + "," +
            VicePresidentRole + "," +
            TreasurerRole + "," +
            GeneralSecretaryRole;

        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roles = new[]
            {
                AdminRole,
                VicePresidentRole,
                TreasurerRole,
                GeneralSecretaryRole,
                MemberRole
            };

            // Eksik roller oluşturulur.
            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        roleResult.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"{roleName} rolü oluşturulamadı: {errors}");
                }
            }

            var adminEmail =
                configuration["AdminAccount:Email"];

            var adminPassword =
                configuration["AdminAccount:Password"];

            var shouldResetAdminPassword =
                configuration.GetValue<bool>(
                    "AdminAccount:ResetPassword");

            // Admin bilgileri tanımlı değilse hesap oluşturulmaz.
            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var adminUser =
                await userManager.FindByEmailAsync(adminEmail);

            // Admin hesabı yoksa oluşturulur.
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult =
                    await userManager.CreateAsync(
                        adminUser,
                        adminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        createResult.Errors.Select(
                            error => error.Description));

                    throw new InvalidOperationException(
                        $"Yönetici hesabı oluşturulamadı: {errors}");
                }
            }
            else if (shouldResetAdminPassword)
            {
                // Ayar açıksa mevcut admin şifresi yenilenir.
                var resetToken =
                    await userManager.GeneratePasswordResetTokenAsync(
                        adminUser);

                var resetResult =
                    await userManager.ResetPasswordAsync(
                        adminUser,
                        resetToken,
                        adminPassword);

                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        resetResult.Errors.Select(
                            error => error.Description));

                    throw new InvalidOperationException(
                        $"Yönetici şifresi sıfırlanamadı: {errors}");
                }
            }

            // Admin e-postası doğrulanmamışsa doğrulanır.
            if (!await userManager.IsEmailConfirmedAsync(adminUser))
            {
                var confirmationToken =
                    await userManager.GenerateEmailConfirmationTokenAsync(
                        adminUser);

                var confirmationResult =
                    await userManager.ConfirmEmailAsync(
                        adminUser,
                        confirmationToken);

                if (!confirmationResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        confirmationResult.Errors.Select(
                            error => error.Description));

                    throw new InvalidOperationException(
                        $"Yönetici e-postası doğrulanamadı: {errors}");
                }
            }

            // Admin rolü henüz atanmamışsa kullanıcıya eklenir.
            if (!await userManager.IsInRoleAsync(
                    adminUser,
                    AdminRole))
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        adminUser,
                        AdminRole);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        roleResult.Errors.Select(
                            error => error.Description));

                    throw new InvalidOperationException(
                        $"Yönetici rolü atanamadı: {errors}");
                }
            }
        }
    }
}