using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class LocationCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public int? ParentId { internal get; set; }
}

public class LocationModel: LocationCreateModel
{
    public string? Id { get; set; }   
}