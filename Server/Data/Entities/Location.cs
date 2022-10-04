using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data.Entities;

[Index(nameof(Slug), IsUnique = true)]
public class Location: Entity
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public string? Notes { get; set; }

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = null!;

    public LocationFlags Flags { get; set; }

    public int Order { get; set; }
    public string? PathData { get; set; }

    public int? ParentId { get; set; }
    public Location? Parent { get; set; }
    
    public ICollection<Location>? Children { get; set; }
    public ICollection<Stuff>? Stuffs { get; set; }
    public ICollection<StuffLocation>? StuffLocations { get; set; }
    public ICollection<Upload>? Uploads { get; set; }
    public ICollection<Tag>? Tags { get; set; }
}

[Flags]
public enum LocationFlags : long
{
    Unset = 0
}