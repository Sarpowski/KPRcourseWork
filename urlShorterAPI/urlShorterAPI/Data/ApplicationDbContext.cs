using Microsoft.EntityFrameworkCore;
using urlShorterAPI.Models;

namespace urlShorterAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<ShortUrl> UrlShorteners { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
