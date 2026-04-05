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
                    new Department { DepartmentId = Guid.NewGuid(), Name = "Human Resources" },
                    new Department { DepartmentId = Guid.NewGuid(), Name = "IT" },
                    new Department { DepartmentId = Guid.NewGuid(), Name = "Finance" },
                    new Department { DepartmentId = Guid.NewGuid(), Name = "Accounting" }
                        
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
