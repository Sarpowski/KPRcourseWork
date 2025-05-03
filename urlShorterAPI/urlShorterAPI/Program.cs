using Microsoft.EntityFrameworkCore;
using System.IO;
using urlShorterAPI.Data;
using urlShorterAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add these lines to your service configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ensure database directory exists
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=urlshortener.db";
var dbPath = connectionString.Replace("Data Source=", "").Split(';')[0];
var dbDirectory = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Use SQLite database
    options.UseSqlite(connectionString);
});

// Configure logging
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Removed HTTPS redirection for Docker compatibility
// app.UseHttpsRedirection();

// Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
// Add this before app.MapShortenerEndPoints();
app.UseCors("AllowAngularApp");

// Map the URL shortener endpoints using extension method
app.MapShortenerEndPoints();

app.Run();