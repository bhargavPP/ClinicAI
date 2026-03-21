using ClinicAI.Application;
using ClinicAI.Application.Features.Doctors.Commands.CreateDoctor;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            // In development allow any origin to ease local testing (including accessing the UI via LAN IP)
            if (builder.Environment.IsDevelopment())
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins("http://localhost:4200", "http://localhost:51912")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateDoctorCommand).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ClinicAI.Application.common.Behaviors.ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(typeof(CreateDoctorValidator).Assembly);
// Add services to the container.
builder.Services.AddDbContext<ClinicDbContext>(options =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();


builder.Services.AddScoped<IClinicDbContext>(Provider => Provider.GetRequiredService<ClinicDbContext>());

var app = builder.Build();
// ✅ 1. Exception middleware FIRST
app.UseMiddleware<ClinicAI.API.Middleware.ExceptionMiddleware>();
// Dev tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Only enable HTTPS redirection in non-development environments to avoid
// redirecting HTTP preflight (OPTIONS) requests during local development.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// ✅ 2. CORS BEFORE auth and endpoints
app.UseCors("AllowAngular");

// ✅ 3. Auth (if used later)
app.UseAuthorization();

// ✅ 4. Endpoints LAST
app.MapControllers();

app.Run();