using ClinicAI.Application.Features.Doctors.Commands.CreateDoctor;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Interface;
using ClinicAI.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// ✅ CORS (FIXED FOR COOKIES)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:51912"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // 🔥 REQUIRED for cookies
    });
});


// ✅ MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ClinicAI.Application.common.AssemblyReference).Assembly));

// ✅ Validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),
    typeof(ClinicAI.Application.common.Behaviors.ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(ClinicAI.Application.common.AssemblyReference).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


// ✅ DB
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClinicDbContext>(provider =>
    provider.GetRequiredService<ClinicDbContext>());


// ✅ Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


// ✅ Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();


// ✅ JWT AUTH (UPDATED FOR COOKIES)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            ClockSkew = TimeSpan.Zero
        };

        // 🔥 IMPORTANT: Read token from COOKIE instead of header
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


var app = builder.Build();


// ✅ 1. Exception middleware FIRST
app.UseMiddleware<ClinicAI.API.Middleware.ExceptionMiddleware>();


// Dev tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Only enable HTTPS redirect in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


// ✅ 2. CORS BEFORE auth
app.UseCors("AllowAngular");


// ✅ 3. AUTH (VERY IMPORTANT ORDER)
app.UseAuthentication();   // 🔥 MUST be here
app.UseAuthorization();


// ✅ 4. Endpoints
app.MapControllers();
await DbInitializer.SeedAdminAsync(app.Services);
app.Run();