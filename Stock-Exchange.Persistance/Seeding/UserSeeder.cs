using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole<Guid>>? roleManager)
        {
            if (userManager == null || roleManager == null)
                return;

            string[] roles = { nameof(UserType.Admin), nameof(UserType.Customer) };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@gmail.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    PhoneNumber = "+201000000001",
                };
                adminUser.UpdateFullName("System Administrator");
                adminUser.ConfirmEmail();
                adminUser.MarkAsCreated("System");

                var result = await userManager.CreateAsync(adminUser, "admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, nameof(UserType.Admin));
                }
            }
        }
    }
}
