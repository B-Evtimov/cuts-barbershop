using Cuts.Api.Data;
using Cuts.Api.Models;
using Cuts.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CutsDbContext>(options =>
    options.UseSqlite("Data Source=cuts.db"));

builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.Encoder =
        System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CUTS Barbershop API", Version = "v1" }));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CutsDbContext>();
    db.Database.EnsureCreated();

    if (!db.Barbers.Any())
    {
        db.Barbers.AddRange(
            new Barber
            {
                Name = "Ivan Petrov",
                Title = "Master Barber · 8 years experience",
                AvatarEmoji = "💈",
                Username = "ivan",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                SatisfactionPercent = 98,
                TotalClients = 1400,
                Rating = 4.9
            },
            new Barber
            {
                Name = "Martin Georgiev",
                Title = "Senior Barber · 5 years experience",
                AvatarEmoji = "✂️",
                Username = "martin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                SatisfactionPercent = 96,
                TotalClients = 900,
                Rating = 4.8
            }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CUTS API v1"));

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();

