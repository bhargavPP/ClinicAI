using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Interface;
using ClinicAI.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicAI.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // ✅ ONE place for DbContext — change here, affects API + Functions
            services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
           
            services.AddScoped<IClinicDbContext, ClinicDbContext>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailLogService, EmailLogService>();
            services.AddScoped<IDateTime, DateTimeService>();

            return services;
        }
    }
}
