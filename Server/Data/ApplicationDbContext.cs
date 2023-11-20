using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Stuff> Stuffs => Set<Stuff>();
    public DbSet<StuffPart> StuffParts => Set<StuffPart>();
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

        builder.Entity<ApplicationUser>()
            .Property(x => x.Settings).HasJsonConversion();

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

        builder.Entity<Stuff>()
            .HasMany(s => s.Parts)
            .WithMany()
            .UsingEntity<StuffPart>
            (
                sl => sl.HasOne(x => x.Parent)
                    .WithMany(s => s.StuffParts)
                    .HasForeignKey(x => x.ParentId),
                sl => sl.HasOne(x => x.Part)
                    .WithMany()
                    .HasForeignKey(x => x.PartId)
                    .OnDelete(DeleteBehavior.Restrict),
                sl => sl.HasKey(x => new { x.ParentId, x.PartId })
            );

        builder.Entity<Stuff>()
            .Property(x => x.Data).HasJsonConversion();

        builder.Entity<Location>()
            .Property(x => x.Data).HasJsonConversion();

        builder.Entity<Purchase>()
            .Property(e => e.Price).HasConversion<double>();

        builder.Entity<PurchaseItem>(e => {
            e.Property(x => x.Price).HasConversion<double>();
            e.Property(x => x.Data).HasJsonConversion();
        });

        builder.Entity<Event>()
            .Property(x => x.Data).HasJsonConversion();

        builder.SeedUsers();
        builder.SeedLocations();
    }
}

internal static class DataSeeder
{
    internal static void SeedUsers(this ModelBuilder builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = "fe73948a-1173-43ad-9473-2f014b39f7c3",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            PasswordHash = hasher.HashPassword(null!, "adminadmin"),
            SecurityStamp = "70effd01-76d5-4d56-85ac-6ddb5ffd3819",
            ConcurrencyStamp = "4ed21d28-faa4-4948-ae7a-e945cb3b75a9"
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
            Created = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            Updated = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
        };
        builder.Entity<Location>().HasData(entity);
    }
}
