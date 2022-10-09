using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Upload: Entity
{
    [Required]
    [MaxLength(255)]
    public string? FileName { get; set; }

    [Required]
    [MaxLength(1023)]
    public string? Path { get; set; }

    // [Required]
    // [MaxLength(1023)]
    // public string? Original { get; set; }

    public string? Notes { get; set; }

    public int Order { get; set; }

    public int? StuffId { get; set; }
    public Stuff? Stuff { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }

    public int? EventId { get; set; }
    public Event? Event { get; set; }
}
