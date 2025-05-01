using Microsoft.EntityFrameworkCore;
using urlShorterAPI.Data;
using urlShorterAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database - you can switch between SQLite or in-memory as needed
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Use SQLite database
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=urlshortener.db");
    
    // Alternatively, use in-memory database for testing
    // options.UseInMemoryDatabase("UrlShortenerDb");
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

app.UseHttpsRedirection();

// Map the URL shortener endpoints using extension method
app.MapShortenerEndPoints();

app.Run();