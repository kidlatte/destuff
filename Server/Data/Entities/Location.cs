using Destuff.Server.Services;
using Destuff.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

[Index(nameof(Slug), IsUnique = true)]
public class Location: Entity, ISluggable
{
    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    public string? Notes { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Slug { get; set; }

    public LocationFlags Flags { get; set; }

    public int Order { get; set; }

    public LocationData? Data { get; set; }

    public int? ParentId { get; set; }
    public Location? Parent { get; set; }
    
    public ICollection<Location>? Children { get; set; }
    public ICollection<Stuff>? Stuffs { get; set; }
    public ICollection<StuffLocation>? StuffLocations { get; set; }
    public ICollection<Upload>? Uploads { get; set; }
    public ICollection<Tag>? Tags { get; set; }

    public string ToSlug() => Name.ToSlug();
}

[Flags]
public enum LocationFlags : long
{
    Unset = 0
}