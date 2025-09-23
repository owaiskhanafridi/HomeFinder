

using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    //It was set traditionally as:
    //public virtual DbSet<Listing> Listings {get; set; }
    //Or in modern C#:
    //public DbSet<Listing> {get; set; } = default!;
    public DbSet<Listing> Listings => Set<Listing>();

    public DbSet<Photo> Photos => Set<Photo>();

    public DbSet<TourRequest> TourRequests => Set<TourRequest>();
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Listing>(e =>
         {
             e.HasIndex(x => x.Title).IsUnique();
             e.Property(x => x.Price).HasPrecision(12, 2);
             e.Property(x => x.Bathrooms).HasPrecision(3, 1);
             e.HasMany(x => x.Photos).WithOne(p => p.Listing).HasForeignKey(p => p.ListingId).OnDelete(DeleteBehavior.Cascade);
             e.HasIndex(x => new { x.City, x.State, x.Price });
             e.HasIndex(x => x.CreatedAt);
         });

        modelBuilder.Entity<Photo>(e => { e.Property(x => x.Url).IsRequired(); });

        modelBuilder.Entity<TourRequest>(e =>
        {
            e.HasOne(t => t.Listing).WithMany().HasForeignKey(t => t.ListingId);
            e.HasIndex(t => new { t.ListingId, t.CreatedAt });
        });

        modelBuilder.Entity<ContactRequest>(e =>
        {
            e.HasOne(c => c.Listing).WithMany().HasForeignKey(c => c.ListingId);
            e.HasIndex(c => new { c.ListingId, c.CreatedAt });
        });
    }
}