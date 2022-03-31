using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Image: Entity
{
    [Required]
    [MaxLength(255)]
    public string? Path { get; set; }

    public string? Notes { get; set; }

    public int Order { get; set; }

    public int? StuffId { get; set; }
    public Stuff? Stuff { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }
}
