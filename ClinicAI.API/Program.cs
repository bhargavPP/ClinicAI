using Azure.Storage.Queues;
using ClinicAI.API.Middleware;
using ClinicAI.Application.common;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Interface;
using ClinicAI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// =========================
// ✅ LOGGING
// =========================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// =========================
// ✅ CORS
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:51912")
                  .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        else
            policy.WithOrigins("https://clinic-ai-ui.azurewebsites.net")
                  .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

// =========================
// ✅ APPLICATION + INFRASTRUCTURE (one line each)
// =========================
builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

// =========================
// ✅ API-ONLY SERVICES
// =========================
builder.Services.AddSingleton<IQueueService, AzureQueueService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// =========================
// ✅ CONTROLLERS + SWAGGER
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================
// ✅ JWT AUTH
// =========================
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var key = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
            Console.WriteLine("❌ JWT Key is NULL");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key ?? "fallback_key")),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"✅ Token validated for: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// =========================
// ✅ HEALTH CHECKS
// =========================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ClinicDbContext>("Database")
    .AddCheck("Self", () => HealthCheckResult.Healthy());

// =========================
// ✅ BUILD APP
// =========================
var app = builder.Build();
Console.WriteLine($"ENVIRONMENT: {builder.Environment.EnvironmentName}");

// =========================
// ✅ DEV TOOLS
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================
// ✅ PIPELINE
// =========================
app.UseCors("AllowAngular");
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("➡️ {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("⬅️ Response: {StatusCode}", context.Response.StatusCode);
});
app.UseMiddleware<ExceptionMiddleware>();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// ✅ ENDPOINTS
// =========================
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message
            })
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
}).AllowAnonymous();

// =========================
// ✅ DB INIT
// =========================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
        db.Database.Migrate();
        await DbInitializer.SeedAdminAsync(app.Services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ DB ERROR: {ex}");
    }
}

// =========================
// ✅ QUEUE INIT
// =========================
var storageConnection =
    builder.Configuration["StorageConnectionString"] ??
    builder.Configuration["AzureQueue:ConnectionString"];

if (!string.IsNullOrEmpty(storageConnection))
{
    try
    {
        var emailQueue = new QueueClient(storageConnection, "email-queue");
        var poisonQueue = new QueueClient(storageConnection, "email-queue-poison");

        await emailQueue.CreateIfNotExistsAsync();
        await poisonQueue.CreateIfNotExistsAsync();

        if (app.Environment.IsDevelopment())
        {
            await emailQueue.ClearMessagesAsync();
            await poisonQueue.ClearMessagesAsync();
            Console.WriteLine("✅ Queues cleared (DEV only)");
        }

        Console.WriteLine($"✅ Queues initialized | ENV: {app.Environment.EnvironmentName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Queue init error: {ex.Message}");
    }
}
else
{
    Console.WriteLine("⚠️ StorageConnectionString missing");
}

app.Run();