
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

public class StuffLocationBasicModel
{
    public required LocationListItem Location { get; set; }
    public int Count { get; set; }
}

public class StuffLocationModel : StuffLocationBasicModel
{
    public required StuffBasicModel Stuff { get; set; }
}
