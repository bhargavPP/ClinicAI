using ClinicAI.API.Middleware;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Interface;
using ClinicAI.Infrastructure.Persistence;
using FluentValidation;
using Hangfire;
using MediatR;
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
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
// =========================
// ✅ CORS (LOCAL + AZURE)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "https://clinic-ai-ui.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =========================
// ✅ MediatR
// =========================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ClinicAI.Application.common.AssemblyReference).Assembly));

// =========================
// ✅ FluentValidation
// =========================
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),
    typeof(ClinicAI.Application.common.Behaviors.ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(typeof(ClinicAI.Application.common.AssemblyReference).Assembly);

// =========================
// ✅ DB Context
// =========================
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHealthChecks().AddDbContextCheck<ClinicDbContext>("Database").AddCheck("Self", () => HealthCheckResult.Healthy());
builder.Services.AddScoped<IClinicDbContext, ClinicDbContext>();

// =========================
// ✅ Services
// =========================
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IQueueService, AzureQueueService>();
builder.Services.AddHttpContextAccessor();

// =========================
// ✅ Controllers
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
        var key = builder.Configuration["JwtKey"];
        Console.WriteLine($"JWT KEY VALUE: {key}");
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("❌ JWT Key is NULL");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key ?? "fallback_key")
            ),

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
// =========================
// ✅ BUILD APP
// =========================
var app = builder.Build();

// =========================
// ✅ DEV TOOLS
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================
// ✅ PIPELINE (CRITICAL ORDER)
// =========================

//  CORS MUST BE FIRST
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
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// =========================
// ✅ ENDPOINTS
// =========================
app.MapControllers();

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
        Console.WriteLine("❌ DB ERROR:");
        Console.WriteLine(ex.ToString());
    }
}
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
app.UseHangfireDashboard();
app.Run();