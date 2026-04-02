using ClinicAI.API.Middleware;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Interface;
using ClinicAI.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// ✅ LOGGING
// =========================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddApplicationInsightsTelemetry();
Console.WriteLine("🚀 Starting app build...");

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
Console.WriteLine("➡️ Configuring DB...");
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClinicDbContext, ClinicDbContext>();

// =========================
// ✅ Services
// =========================
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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
Console.WriteLine("➡️ Configuring JWT...");

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

// =========================
// ✅ BUILD APP
// =========================
var app = builder.Build();

Console.WriteLine("✅ App build completed");

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
Console.WriteLine("➡️ Applying middleware...");

// 🔥 CORS MUST BE FIRST
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
Console.WriteLine("➡️ Mapping endpoints...");

app.MapControllers();
 
// =========================
// ✅ DB INIT
// =========================
using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("➡️ DB initialization started");

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

        db.Database.Migrate();
        await DbInitializer.SeedAdminAsync(app.Services);

        Console.WriteLine("✅ DB initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ DB ERROR:");
        Console.WriteLine(ex.ToString());
    }
}

// =========================
// ✅ RUN
// =========================
Console.WriteLine("🔥 Starting app...");
app.Run();