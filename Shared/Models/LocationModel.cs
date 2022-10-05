using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ILocationModel
{
    string? Id { get; set; }
    string? Slug { get; set; }
    string? Name { get; set; }
}

public class LocationCreateModel
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    public string? Notes { get; set; }

    public string? ParentId { get; set; }
}

public class LocationModel: LocationCreateModel, ILocationModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public LocationDataModel? Data { get; set; }

    public List<LocationModel>? Children { get; set; }
}

public class LocationBasicModel: ILocationModel
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Name { get; set; }
}

public class LocationTreeModel : LocationBasicModel
{
    public List<LocationTreeModel>? Children { get; set; }
}

public class LocationDataModel
{
    public ICollection<LocationBasicModel>? Path { get; set; }
}
