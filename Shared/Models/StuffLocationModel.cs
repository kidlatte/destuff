
namespace Destuff.Shared.Models;

public class StuffLocationRequest
{
    public required string StuffId { get; set; }
    public string? LocationId { get; set; }
    public int Count { get; set; }
}

public class StuffLocationModel
{
    public required LocationListItem Location { get; set; }
    public required StuffBasicModel Stuff { get; set; }
    public int Count { get; set; }
}
