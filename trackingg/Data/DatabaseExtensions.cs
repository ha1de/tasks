using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using trackingg.Data;


    public static class DatabaseExtensions
    {
        public static async Task EnsureDatabaseCreatedAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedRolesAsync(roleManager);
                await SeedAdminUserAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while creating/seeding the database.");
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Roles are already seeded in the OnModelCreating method,
            // but we need to check if they exist in the database
            string[] roleNames = { "Admin", "ProjectManager", "Developer", "Tester", "Guest" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Create a default admin user
            var adminUser = await userManager.FindByEmailAsync("admin@trackingg.com");
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@trackingg.com",
                    Email = "admin@trackingg.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    DateJoined = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }

