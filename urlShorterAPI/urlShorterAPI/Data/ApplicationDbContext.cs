using Microsoft.EntityFrameworkCore;
using urlShorterAPI.Models;

namespace urlShorterAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ShortUrl> UrlShorteners { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShortUrl>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalUrl).IsRequired();
            entity.Property(e => e.ShortenUrl).IsRequired();
            entity.HasIndex(e => e.ShortenUrl).IsUnique();
        });
    }
}