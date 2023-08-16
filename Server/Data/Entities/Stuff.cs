using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data.Entities;

[Index(nameof(Slug), IsUnique = true)]
public class Stuff: Entity
{
    [Required]
    [MaxLength(1023)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(1023)]
    public required string Slug { get; set; }

    public string? Url { get; set; }

    public string? Notes { get; set; }

    public StuffFlags Flags { get; set; }

    public int Order { get; set; }
    
    public int Count { get; set; }
    public DateTime? Computed { get; set; }
    public DateTime? Inventoried { get; set; }

    public ICollection<Location>? Locations { get; set; }
    public ICollection<StuffLocation>? StuffLocations { get; set; }
    public ICollection<PurchaseItem>? PurchaseItems { get; set; }
    public ICollection<Upload>? Uploads { get; set; }
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<Event>? Events { get; set; }
}

[Flags]
public enum StuffFlags : long
{
    Unset = 0
}