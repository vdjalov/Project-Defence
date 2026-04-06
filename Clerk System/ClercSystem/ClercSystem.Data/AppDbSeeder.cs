using ClercSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClercSystem.Data
{
    public static class AppDbSeeder
    {
        // initial department seeder
        public static async Task SeedDepartmentsAsync(IServiceProvider serviceProvider) 
        {
            AppDbContext context = serviceProvider.GetRequiredService<AppDbContext>();

            if (! await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department {  Name = "Human Resources" , Location="Plovdiv" },
                    new Department {  Name = "IT", Location = "Varna" }, 
                    new Department {  Name = "Finance", Location= "Sofia" },
                    new Department {  Name = "Accounting", Location = "Stara Zagora" }
                        
                );

                await context.SaveChangesAsync();
            }
        }

        // initial categories seeder
        public static async Task SeedCategoriesAsync(IServiceProvider serviceProvider)
        {
            AppDbContext context = serviceProvider.GetRequiredService<AppDbContext>();

            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(
                    new Category { CategoryName = "Administrative", Description = "Built Permit for a house" },
                    new Category { CategoryName = "Financial", Description = "Invoice Itd Company" },
                    new Category { CategoryName = "Legal", Description = "Warrant for illegal waste!" },
                    new Category { CategoryName = "Technical", Description = "Issues with legal Informational system" }

                );

                await context.SaveChangesAsync();
            }
        }
    }
}
