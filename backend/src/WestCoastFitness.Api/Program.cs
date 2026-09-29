using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Data;
using WestCoastFitness.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("FitnessClubDb")
    ?? "Host=localhost;Port=5432;Database=westcoastfitness;Username=postgres;Password=postgres";

builder.Services.AddDbContext<FitnessClubDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

public partial class Program;
