using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Stuff: Entity
{
    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public StuffFlags Flags { get; set; }

    public int Order { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public ICollection<Image>? Images { get; set; }
    public ICollection<Tag>? Tags { get; set; }
}

[Flags]
public enum StuffFlags : long
{
    Unset = 0
}