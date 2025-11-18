using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Settings
{
    public static class RoleValidator
    {
        public static async Task ValidateRolesAsync(
            RoleManager<IdentityRole<int>> roleManager,
            ILogger logger)
        {
            string[] expectedRoles = { "Admin", "RestaurantOwner", "RestaurantEmployee", "DeliveryPerson", "User" };

            foreach (var roleName in expectedRoles)
            {
                var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                {
                    logger.LogError("Role {RoleName} is missing from the database!", roleName);
                }
                else if (role.Id <= 0)
                {
                    logger.LogError("Role {RoleName} has an invalid ID {RoleId}", roleName, role.Id);
                }
                else
                {
                    logger.LogInformation("Role {RoleName} exists with ID {RoleId}", roleName, role.Id);
                }
            }
        }
    }
}