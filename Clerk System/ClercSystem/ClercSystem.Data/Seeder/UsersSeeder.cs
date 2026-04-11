using ClercSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ClercSystem.Data.Seeder
{
    public static class UsersSeeder
    {
       

        public static async Task SeedAdminDefaultUsersAsync(IServiceProvider serviceProvider, Guid departmentId)
        {

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
           

            //Check if admin user exists
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null) // Admin user already exists, ensure it has the Admin role
            {
                var roleExists = await roleManager.RoleExistsAsync("Admin");

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
                }

                var userRoleAdded = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (userRoleAdded.Succeeded) // Ensure the user has the Admin role
                {
                    Console.WriteLine("Admin user created successfully with Admin role.");
                }
                else
                {
                    Console.WriteLine("Admin user created but failed to assign Admin role.");
                }

                return;
            }

            var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                IsManager = true,
                DepartmentId = departmentId
            };

            var result = await userManager.CreateAsync(user, "Admin@123");

            if(result.Succeeded)
            {
                var userRoleAdded = await userManager.AddToRoleAsync(user, "Admin");
                if(userRoleAdded.Succeeded)
                {
                    Console.WriteLine("Admin user created successfully with Admin role.");
                }
                else
                {
                    Console.WriteLine("Admin user created but failed to assign Admin role.");
                }
            }

            // Adding claims so that i can get user confition through it
            await userManager.AddClaimAsync(user, new Claim("IsManager", user.IsManager.ToString()));

              
        }
    }
}
