using Microsoft.AspNetCore.Identity;
using static CoreTemplate.Helpers.Constants.Enumerations;

namespace CoreTemplate.Data.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                foreach (var role in Enum.GetValues(typeof(Roles)))
                {
                    await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
        }
    }
}
