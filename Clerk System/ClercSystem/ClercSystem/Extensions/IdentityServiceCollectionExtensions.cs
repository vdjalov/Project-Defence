
using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Implementations;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Implementations;
using ClercSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        // Get db services and connection
        public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }



        // Adding repositories and services with a  method
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration congfig)
        {
           
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentUserRepository, DocumentUserRepository>();
            services.AddScoped<IDocumentLogsRepository, DocumentLogsRepository>();

            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IMyDocumentUserService, MyDocumentUserService>();

            return services;
        }


        public static IServiceCollection AddApplicationIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                //SignIn settings
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Password Settings
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                //User Settings
                options.User.RequireUniqueEmail = true;

                //Lockout Settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;


            })
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders()
               .AddDefaultUI();

            return services;
        }

    }
}
