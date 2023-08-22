using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class InventoryRequest
{
    [Required]
    public required string StuffId { get; set; }
    public int Count { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }
}
