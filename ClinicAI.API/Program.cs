using ClinicAI.Application;
using ClinicAI.Application.Features.Doctors.Commands.CreateDoctor;
using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:51912")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateDoctorCommand).Assembly));
// Add services to the container.
builder.Services.AddDbContext<ClinicDbContext>(options =>
                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();


builder.Services.AddScoped<IClinicDbContext>(Provider => Provider.GetRequiredService<ClinicDbContext>());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ClinicAI.Application.common.Behaviors.ValidationBehavior<,>));
var app = builder.Build();

// Dev tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ✅ 1. Exception middleware FIRST
app.UseMiddleware<ClinicAI.API.Middleware.ExceptionMiddleware>();

// ✅ 2. CORS BEFORE auth and endpoints
app.UseCors("AllowAngular");

// ✅ 3. Auth (if used later)
app.UseAuthorization();

// ✅ 4. Endpoints LAST
app.MapControllers();

app.Run();