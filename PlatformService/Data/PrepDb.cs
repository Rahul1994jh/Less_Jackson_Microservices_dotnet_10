using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>() ?? throw new InvalidOperationException("Failed to get AppDbContext from service provider");
            SeedData(context, isProduction);
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                System.Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding new platforms...");
                context.Platforms.AddRange(
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                Console.WriteLine("--> New platforms added");
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}