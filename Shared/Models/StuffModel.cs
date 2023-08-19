using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Destuff.Shared.Services;

namespace Destuff.Shared.Models;

public interface IStuffModel
{
    public string Id { get; set; }
    public string Slug { get; set; }
    public string? Name { get; set; }
}

public class StuffRequest : IRequest
{
    [Required]
    [StringLength(1023)]
    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Notes { get; set; }

    public string? LocationId { get; set; }
}

public class StuffModel : IModel, IStuffModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }

    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Notes { get; set; }

    public required ICollection<StuffLocationModel> StuffLocations { get; set; }
    public ICollection<UploadModel>? Uploads { get; set; }

    [JsonIgnore]
    public LocationListItem? FirstLocation => StuffLocations.FirstOrDefault()?.Location;

    [JsonIgnore]
    public bool IsSingleLocation 
    {
        get 
        {
            var count = StuffLocations.Count;
            return count == 0 || count == 1 && StuffLocations.First().Count == 1;
        }
    }

    public StuffRequest ToRequest()
    {
        return new StuffRequest
        {
            Name = Name,
            Url = Url,
            Notes = Notes,
            LocationId = IsSingleLocation ? FirstLocation?.Id : null
        };
    }
}

public class StuffListItem : IStuffModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }
    public string? Name { get; set; }
    public ICollection<LocationListItem>? Locations { get; set; }
}

public class StuffBasicModel : IStuffModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }
    public string? Name { get; set; }
}