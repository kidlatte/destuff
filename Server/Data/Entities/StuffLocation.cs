using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data.Entities;

// [Index(nameof(StuffId), nameof(LocationId), IsUnique = true)]
public class StuffLocation
{
    public int StuffId { get; set; }
    public Stuff Stuff { get; set; } = null!;

    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public int Count { get; set; }
}