using ClinicAI.Functions.Services;
using ClinicAI.Infrastructure.Persistence;
using ClinicAI.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpContextAccessor();
        // ✅ Register here — not in the API
        services.AddInfrastructure(context.Configuration);

        // ✅ Functions-only — SMTP email implementation lives here
        services.AddScoped<IEmailService, EmailService>(); 
    })
    .Build();

await host.RunAsync();
