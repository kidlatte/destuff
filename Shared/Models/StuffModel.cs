using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    public ICollection<UploadModel>? Uploads { get; set; }

    public LocationListItem? FirstLocation => StuffLocations?.FirstOrDefault()?.Location;

    [JsonIgnore]
    public bool IsSingleLocation 
    {
        get 
        {
            var count = StuffLocations.OrEmpty().Count();
            return count == 0 || count == 1 && StuffLocations?.First().Count == 1;
        }
    }

    public StuffCreateModel ToCreate()
    {
        return new StuffCreateModel
        {
            Name = Name,
            Notes = Notes,
            LocationId = IsSingleLocation ? FirstLocation?.Id : null
        };
    }
}

public class StuffListModel : IStuffModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
    public ICollection<LocationListItem>? Locations { get; set; }
}