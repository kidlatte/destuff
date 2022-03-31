using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Tag: Entity
{
    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Slug { get; set; }

    public ICollection<Stuff>? Stuff { get; set; }
    public ICollection<Location>? Location { get; set; }
}
