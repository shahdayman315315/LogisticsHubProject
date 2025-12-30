using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Identity.Core;
namespace LogisticsHub.Infrastructure.Extensions
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequiredLength = 8; // كفاية دي بدل Length

                // Lockout settings - لاحظي الحروف الكبيرة والسبيلنج
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // يفضل دقائق مش ثواني
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

            })
            .AddEntityFrameworkStores<AppDbContext>() // ضفنا الأقواس ()
            .AddDefaultTokenProviders(); // صححنا السبيلنج وضفنا الأقواس ()

            return services;
        }
    }
}
