using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public class Supplier : Entity
{
    [Required]
    [MaxLength(255)]
    public string ShortName { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Notes { get; set; }

    public ICollection<Purchase>? Purchases { get; set; }

}