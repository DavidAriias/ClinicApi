using System;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

using ClinicApi.Infrastructure.Context;
using ClinicApi.Infraestructure.Repositories;
using ClinicApi.Domain.Repositories;
using ClinicApi.App.Services;
using ClinicApi.App.Common;
using ClinicApi.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<DoctorService>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;

    while (retries > 0)
    {
        try
        {
            Console.WriteLine("Applying migrations...");
            db.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Waiting for SQL Server... {ex.Message}");

            if (retries == 0)
                throw;

            Thread.Sleep(5000);
        }
    }
}

// Pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();