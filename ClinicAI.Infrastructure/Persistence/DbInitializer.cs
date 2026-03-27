using ClinicAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicAI.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

            await context.Database.MigrateAsync();

            // ✅ Check if admin already exists
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new User
            {
                id = Guid.NewGuid(),
                Email = "admin@test.com",
                FullName = "Admin User",
                Phone = "1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
