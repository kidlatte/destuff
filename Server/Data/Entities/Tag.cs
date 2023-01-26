using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data.Entities;

[Index(nameof(Slug), IsUnique = true)]
public class Tag: Entity
{
    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(255)]
    public required string Slug { get; set; }

    public ICollection<Stuff>? Stuff { get; set; }
    public ICollection<Location>? Location { get; set; }
}
