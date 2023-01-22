using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Data.Entities;

public class StuffLocation
{
    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public int Count { get; set; }
}