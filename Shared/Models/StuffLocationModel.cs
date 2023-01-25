
namespace Destuff.Shared.Models;

public class StuffLocationCreateModel
{
    public string? StuffId { get; set; }
    public string? LocationId { get; set; }
    public int Count { get; set; }
}

public class StuffLocationModel
{
    public LocationListItem? Location { get; set; }
    public int Count { get; set; }
}