using LogisticsHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsHub.Infrastructure.Data
{
    public static class DataSeeder
    {
        
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>() { "Admin", "Merchant", "Customer" };
            
            foreach (var role in roles)
            {
                var result=await roleManager.RoleExistsAsync(role);

                if (!result)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@315";

            var existAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existAdmin is null)
            {
                var adminUser = new ApplicationUser()
                {
                    UserName = "AdminUser",
                    FullName = "AdminUser",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,

                };
                var result = await userManager.CreateAsync(adminUser,adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            
            
        }

    }
}
