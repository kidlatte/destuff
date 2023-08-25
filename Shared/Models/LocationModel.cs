using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ILocationModel: IModel
{
    string Slug { get; set; }
    string Name { get; set; }
}

public interface ILocationDataModel : ILocationModel
{
    LocationData? Data { get; set; }
}

public class LocationRequest: IRequest
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? ParentId { get; set; }
}

public class LocationModel: ILocationDataModel
{
    public required string Id { get; set; }
    public string? ParentId { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string? Notes { get; set; }

    public LocationData? Data { get; set; }

    public List<LocationModel>? Children { get; set; }

    public LocationRequest ToRequest()
    {
        return new LocationRequest
        {
            Name = Name,
            ParentId = ParentId,
            Notes = Notes,
        };
    }
}

public class LocationListItem: ILocationModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string? PathString { get; set; }
}

public class LocationTreeModel : LocationListItem
{
    public List<LocationTreeModel>? Children { get; set; }
}

public class LocationLookupItem : LocationListItem, ILocationDataModel
{
    public LocationData? Data { get; set; }
}

public class LocationData
{
    public string? PathString { get; set; }
    public ICollection<LocationListItem>? Path { get; set; }

    public LocationData()
    {
    }

    public LocationData(string name, ICollection<LocationListItem>? path = null)
    {
        Path = path ?? new List<LocationListItem>();
        PathString = string.Join(" > ", Path.Select(x => x.Name).Append(name));
    }
}
