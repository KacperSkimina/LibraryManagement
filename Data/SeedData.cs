using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Utwórz role jeśli nie istnieją
            string[] roleNames = { "Administrator", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Utwórz admina jeśli nie istnieje
            var adminEmail = "admin@biblioteka.pl";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }

            // Utwórz zwykłego użytkownika
            var userEmail = "user@biblioteka.pl";
            var normalUser = await userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                normalUser = new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(normalUser, "User123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }
}