using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ILocationModel
{
    string? Id { get; set; }
    string? Slug { get; set; }
    string? Name { get; set; }
}

public interface ILocationDataModel : ILocationModel
{
    LocationData? Data { get; set; }
}

public class LocationCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? ParentId { get; set; }
}

public class LocationModel: LocationCreateModel, ILocationDataModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public LocationData? Data { get; set; }

    public List<LocationModel>? Children { get; set; }
}

public class LocationListItem: ILocationModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
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
    public ICollection<LocationListItem>? Path { get; set; }
}
