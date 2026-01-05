
using LogisticsHub.Domain.Entities;
using LogisticsHub.Infrastructure.Data;
using LogisticsHub.Infrastructure.Extensions;
using LogisticsHub.Presentation.Middlewares;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LogisticsHub.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            using(var scope=app.Services.CreateScope())
            {
                var UserManager=scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var RoleManager=scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await DataSeeder.SeedRolesAsync(RoleManager);
                await DataSeeder.SeedAdminAsync(UserManager);

            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
