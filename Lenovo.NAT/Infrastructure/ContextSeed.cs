using Lenovo.NAT.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Lenovo.NAT.Infrastructure
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            try
            {
                var defaultUser = new User
                {
                    Name = "Tiago Morine Baganha",
                    NetworkId = "tbaganha",
                    Position = "Senior Software Developer",
                    Department = "Business Development",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddHours(-3),
                    CreatedBy = "system",

                    UserName = "tbaganha",
                    Email = "tbaganha@lenovo.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    //Id = "540B2C53-949E-4525-99D4-290C9C741BA0"
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        var result = await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                            var code = await userManager.GenerateEmailConfirmationTokenAsync(defaultUser);

                        }
                    }
                    await roleManager.SeedClaimsForSuperAdmin();
                } 
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("SuperAdmin");
            await roleManager.AddPermissionClaim(adminRole, "Users");
        }
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }

    public enum Roles
    {
        SuperAdmin,
        Admin,
        Basic,
    }
}
