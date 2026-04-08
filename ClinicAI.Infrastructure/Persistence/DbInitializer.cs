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
            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@test.com",
                    FullName = "Admin User",
                    Phone = "1234567890",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                };

                context.Users.Add(admin);
            }

            // ✅ Create Doctor if not exists
            if (!await context.Users.AnyAsync(u => u.Role == "Doctor"))
            {
                var doctor = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "doctor@test.com",
                    Phone = "1234567890",
                    FullName = "Doctor User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Doctor"
                };

                context.Users.Add(doctor);
            }

            await context.SaveChangesAsync();
        }
    }
}
