using ClinicAI.Application.Interfaces;
using ClinicAI.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
