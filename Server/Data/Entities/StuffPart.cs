namespace Destuff.Server.Data.Entities;

public class StuffPart
{
    public int ParentId { get; set; }
    public Stuff? Parent { get; set; }

    public int PartId { get; set; }
    public Stuff? Part { get; set; }

    public int Count { get; set; }
}
