using Microsoft.EntityFrameworkCore;
using url_shortener.core.Entities;

namespace url_shortener.api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ShortenedUrl
        modelBuilder.Entity<ShortenedUrl>()
            .ToTable("shortened_urls");
        modelBuilder.Entity<ShortenedUrl>()
            .Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<ShortenedUrl>()
            .Property(u => u.Url).HasColumnName("url");
        modelBuilder.Entity<ShortenedUrl>()
            .Property(u => u.ShortCode).HasColumnName("short_code");
        modelBuilder.Entity<ShortenedUrl>()
            .Property(u => u.CreatedAt).HasColumnName("created_at")
            .HasColumnType("timestamptz");
        modelBuilder.Entity<ShortenedUrl>()
            .Property(u => u.UpdatedAt).HasColumnName("updated_at")
            .HasColumnType("timestamptz");
        
        // ShortenedUrlVisit
        modelBuilder.Entity<ShortenedUrlVisit>()
            .ToTable("shortened_url_visits");
        modelBuilder.Entity<ShortenedUrlVisit>()
            .Property(u => u.Id).HasColumnName("id");
        modelBuilder.Entity<ShortenedUrlVisit>()
            .Property(u => u.IpAddress).HasColumnName("ip_address");
        modelBuilder.Entity<ShortenedUrlVisit>()
            .Property(u => u.DateAccessed).HasColumnName("created_at");
        modelBuilder.Entity<ShortenedUrlVisit>()
            .Property(u => u.ShortenedUrlId).HasColumnName("shortened_url_id_fk");
        
        // Relationships
        modelBuilder.Entity<ShortenedUrl>()
            .HasMany(u => u.Visits)
            .WithOne(v => v.ShortenedUrl)
            .HasForeignKey(v => v.ShortenedUrlId)
            .HasPrincipalKey(s => s.Id);
    }
    
    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
    public DbSet<ShortenedUrlVisit> ShortenedUrlVisits { get; set; }
}