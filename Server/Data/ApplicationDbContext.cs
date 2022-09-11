using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Destuff.Server.Data.Entities;

namespace Destuff.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Stuff> Stuffs => Set<Stuff>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Image> Images => Set<Image>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
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
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        builder.Entity<ApplicationUser>().HasData(adminUser);
    }

    internal static void SeedLocations(this ModelBuilder builder)
    {
        var entity = new Location
        {
            Id = 1,
            Name = "Office",
            Slug = "office",
            CreatedBy = "admin",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
        };
        builder.Entity<Location>().HasData(entity);
    }
}
