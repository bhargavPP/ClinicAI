using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClinicAI.Infrastructure.Persistence
{
    public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
    {
        public ClinicDbContext CreateDbContext(string[] args)
        {
            // Walk up to solution root and read appsettings from API project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(),
                "../ClinicAI.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ClinicDbContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            // Pass mock/no-op services for design-time only
            return new ClinicDbContext(
                optionsBuilder.Options,
                new NoOpCurrentUserService(),
                new NoOpDateTimeService()
            );
        }
    }

    // Design-time stubs (no real logic needed)
    file class NoOpCurrentUserService : ClinicAI.Application.Interfaces.ICurrentUserService
    {
        public Guid UserId => Guid.Empty;
        // implement any other members your interface requires
    }

    file class NoOpDateTimeService : ClinicAI.Application.Interfaces.IDateTime
    {
        public DateTime dateTimeUtcNow => DateTime.UtcNow;
        // implement any other members your interface requires
    }
}
