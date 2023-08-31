using CoreTemplate.Helpers.Constants;
using CoreTemplate.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static CoreTemplate.Helpers.Constants.Enumerations;

namespace CoreTemplate.Data.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedSuperAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var superAdmin = new ApplicationUser
            {
                LastName = "ADMIN",
                FirstName = "Super",
                Email = "superadmin@admin.com",
                EmailConfirmed = true,
                UserName = "superadmin"
            };

            var user = await userManager.FindByNameAsync(superAdmin.UserName) ?? await userManager.FindByEmailAsync(superAdmin.Email);

            if (user == null)
            {
                await userManager.CreateAsync(superAdmin, "P@ssword123");
                await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin.ToString());
            }

            //Add Role Claims For SuperAdmin User
            await roleManager.SeedClaimsForSuperAdminUser();
        }

        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager/*, RoleManager<IdentityRole> roleManager*/)
        {
            var admin = new ApplicationUser
            {
                LastName = "ADMIN",
                FirstName = "Admin",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                UserName = "admin"
            };

            var user = await userManager.FindByNameAsync(admin.UserName) ?? await userManager.FindByEmailAsync(admin.Email);

            if (user == null)
            {
                await userManager.CreateAsync(admin, "P@ssword123");
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }

            // TO DO : Add Role Claims For Admin User
        }

        public static async Task SeedBasicUserAsync(UserManager<ApplicationUser> userManager/*, RoleManager<IdentityRole> roleManager*/)
        {
            var basic = new ApplicationUser
            {
                LastName = "USER",
                FirstName = "Basic",
                Email = "basic@user.com",
                EmailConfirmed = true,
                UserName = "basic"
            };

            var user = await userManager.FindByNameAsync(basic.UserName) ?? await userManager.FindByEmailAsync(basic.Email);

            if (user == null)
            {
                await userManager.CreateAsync(basic, "P@ssword123");
                await userManager.AddToRoleAsync(basic, Roles.BasicUser.ToString());
            }
        }
        
        private static async Task SeedClaimsForSuperAdminUser(this RoleManager<IdentityRole> roleManager)
        {
            var saRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(saRole, "Products");
        }

        public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
