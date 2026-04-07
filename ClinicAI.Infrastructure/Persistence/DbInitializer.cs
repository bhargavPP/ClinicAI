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
            if (await context.Users.AnyAsync(u => u.Role == "Doctor"))
                return;
            // ✅ Check if admin already exists
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@test.com" ,
                FullName = "Admin User",
                Phone = "1234567890",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            };
            var doctor = new User
            {
                Id = Guid.NewGuid(),
                Email = "doctor@test.com",
                Phone = "1234567890",
                FullName = "Doctor User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Doctor"

            };       //var admin = new User
            //{
            //    Id = Guid.NewGuid(),
            //    Email = "patient@test.com",
            //    FullName = "patient User",
            //    Phone = "1234567890",
            //    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pat@123"),
            //    Role = "patient"
            //};
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            context.Users.Add(doctor);
            await context.SaveChangesAsync();
        }
    }
}
