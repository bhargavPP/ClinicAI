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
// ✅ CORS (Angular + Cookies)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:51912"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // 🔥 REQUIRED for cookies
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
builder.Services.AddOpenApi();


// =========================
// ✅ JWT AUTH (COOKIE BASED)
// =========================
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            ClockSkew = TimeSpan.Zero
        };

        // 🔥 READ TOKEN FROM COOKIE
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


// =========================
// ✅ GLOBAL EXCEPTION HANDLER
// =========================
app.UseMiddleware<ClinicAI.API.Middleware.ExceptionMiddleware>();


// =========================
// ✅ DEV TOOLS
// =========================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


// =========================
// ✅ HTTPS (ONLY PROD)
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


// =========================
// ✅ CORS (MUST BE BEFORE AUTH)
// =========================
app.UseCors("AllowAngular");


// =========================
// ✅ AUTH (ORDER MATTERS)
// =========================
app.UseAuthentication();
app.UseAuthorization();


// =========================
// ✅ ENDPOINTS
// =========================
app.MapControllers();


// =========================
// ✅ SEED DATA
// =========================
await DbInitializer.SeedAdminAsync(app.Services);


// =========================
// ✅ RUN
// =========================
app.Run();