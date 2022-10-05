using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;

namespace Destuff.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Stuff> Stuffs => Set<Stuff>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<StuffLocation> StuffLocations => Set<StuffLocation>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Upload> Uploads => Set<Upload>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Event> Events => Set<Event>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Stuff>()
            .HasMany(s => s.Locations)
            .WithMany(l => l.Stuffs)
            .UsingEntity<StuffLocation>
            (
                sl => sl.HasOne(x => x.Location)
                    .WithMany(l => l.StuffLocations)
                    .HasForeignKey(x => x.LocationId),
                sl => sl.HasOne(x => x.Stuff)
                    .WithMany(s => s.StuffLocations)
                    .HasForeignKey(x => x.StuffId),
                sl => sl.HasKey(x => new { x.StuffId, x.LocationId })
            );

        builder.Entity<Location>()
            .Property(x => x.Data).HasJsonConversion();

        builder.SeedUsers();
        builder.SeedLocations();
    }
}

internal static class DataSeeder
{
    internal static void SeedUsers(this ModelBuilder builder)
    {
        var hasher = new PasswordHasher<ApplicationUser?>();
        var adminUser = new ApplicationUser
        {
            Id = "fe73948a-1173-43ad-9473-2f014b39f7c3",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            PasswordHash = hasher.HashPassword(null, "adminadmin"),
            SecurityStamp = "70effd01-76d5-4d56-85ac-6ddb5ffd3819",
        };
        builder.Entity<ApplicationUser>().HasData(adminUser);
    }

    internal static void SeedLocations(this ModelBuilder builder)
    {
        var entity = new Location
        {
            Id = 1,
            Name = "Storage",
            Slug = "storage",
            CreatedBy = "admin",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
        };
        builder.Entity<Location>().HasData(entity);
    }
}
