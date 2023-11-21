
using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class StuffLocationRequest
{
    [Required]
    public string? StuffId { get; set; }

    [Required]
    public string? LocationId { get; set; }
    public int Count { get; set; }
}

public class StuffLocationListItem
{
    public required LocationListItem Location { get; set; }
    public int Count { get; set; }
}

public class StuffLocationModel : StuffLocationListItem
{
    public required StuffBasicModel Stuff { get; set; }
}
