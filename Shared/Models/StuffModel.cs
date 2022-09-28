using System.ComponentModel.DataAnnotations;
using Destuff.Shared.Services;

namespace Destuff.Shared.Models;

public class StuffCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? LocationId { get; set; }
}

public class StuffModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }

    public string? Name { get; set; }
    public string? Notes { get; set; }

    public ICollection<LocationBasicModel>? Locations { get; set; }

    public StuffCreateModel ToCreate() 
    { 
        return new StuffCreateModel
        {
            Name = Name,
            Notes = Notes,
            LocationId = Locations.OrEmpty().Count() > 1 ? null : Locations?.FirstOrDefault()?.Id
        };
    }
}