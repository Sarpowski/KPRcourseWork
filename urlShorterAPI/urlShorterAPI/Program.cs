using Microsoft.EntityFrameworkCore;
using System.IO;
using urlShorterAPI.Data;
using urlShorterAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // policy.WithOrigins("http://localhost:4200") // Angular dev server
        //     .AllowAnyHeader()
        //     .AllowAnyMethod();
        policy.WithOrigins(
                        "http://localhost:4200",  // Angular dev server
                        "http://localhost:80",    // Angular in Docker
                        "http://urlshortener-client", // Docker service name
                        "http://localhost"        // Just localhost
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
       
        
    });
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// database directory exists
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
    //SQLite database
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


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
app.UseCors("AllowAngularApp");

app.MapShortenerEndPoints();

app.Run();