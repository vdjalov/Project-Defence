using ClercSystem.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Data
{
    public static class AppDbSeeder
    {
        public static async Task SeedDepartmentsAsync(IServiceProvider serviceProvider)
        {
            AppDbContext context = serviceProvider.GetRequiredService<AppDbContext>();

            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department {  Name = "Human Resources" },
                    new Department {  Name = "IT" },
                    new Department {  Name = "Finance" },
                    new Department {  Name = "Accounting" }
                        
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
