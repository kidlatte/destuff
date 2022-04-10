using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class LocationCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? ParentId { get; set; }
}

public class LocationModel: LocationCreateModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }

    // public LocationModel? Parent { get; set; }
    public List<LocationModel>? Children { get; set; }
}