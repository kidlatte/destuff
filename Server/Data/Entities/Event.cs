namespace Destuff.Server.Data.Entities;

public abstract class EventItem : Entity
{
    public EventType Type { get; set; }

    public DateTime DateTime { get; set; }
    public int Count { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }

    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }

}

public enum EventType
{
    Purchase,
    Inventory,
    Event,
    Move,
    Dispose
}

public class Event : EventItem
{
 
    public int LocationId { get; set; }
    public Location? Location { get; set; }
   
    public ICollection<Upload>? Uploads { get; set; }

    public int ToLocationId { get; set; }
    public Location? ToLocation { get; set; }
}
