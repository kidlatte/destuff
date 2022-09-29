using System.ComponentModel.DataAnnotations;
using Destuff.Shared.Services;

namespace Destuff.Shared.Models;

public interface IStuffModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
}

public class StuffCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? LocationId { get; set; }
}

public class StuffModel : IStuffModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }

    public string? Name { get; set; }
    public string? Notes { get; set; }

    public ICollection<StuffLocationModel>? StuffLocations { get; set; }

    public LocationBasicModel? FirstLocation => StuffLocations?.FirstOrDefault()?.Location;

    public bool IsSingleLocation 
    {
        get 
        {
            var count = StuffLocations.OrEmpty().Count();
            return count == 0 || count == 1 && StuffLocations?.First().Count == 1;
        }
    }

}

public class StuffListModel : IStuffModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public ICollection<LocationBasicModel>? Locations { get; set; }
}