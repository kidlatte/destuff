using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class StuffCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public int Order { get; set; }

    public string? LocationId { get; set; }
}

public class StuffModel : StuffCreateModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public LocationModel? Location { get; set; }
}