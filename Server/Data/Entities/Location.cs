using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Location: Entity
{
    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public LocationFlags Flags { get; set; }

    public int Order { get; set; }

    public int? ParentId { get; set; }
    public Location? Parent { get; set; }
    
    public ICollection<Location>? Children { get; set; }
    public ICollection<Stuff>? Stuffs { get; set; }
    public ICollection<Image>? Images { get; set; }
    public ICollection<Tag>? Tags { get; set; }
}

[Flags]
public enum LocationFlags : long
{
    Unset = 0
}