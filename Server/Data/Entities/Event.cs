using Destuff.Shared.Models;

namespace Destuff.Server.Data.Entities;

public interface IEvent
{
    EventType Type { get; set; }

    DateTime DateTime { get; set; }
    int Count { get; set; }
    string? Summary { get; set; }
    string? Notes { get; set; }

    int StuffId { get; set; }
    Stuff Stuff { get; set; }
}

public enum EventType
{
    Purchase,
    Inventory,
    Event,
    Move,
    Lend,
    Dispose
}

public class Event : Entity, IEvent
{
    public EventType Type { get; set; }

    public DateTime DateTime { get; set; }
    public int Count { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }

    public EventData? Data { get; set; }

    public int StuffId { get; set; }
    public required Stuff Stuff { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }
   
    public ICollection<Upload>? Uploads { get; set; }

    public int ToLocationId { get; set; }
    public Location? ToLocation { get; set; }
}

public class EventData
{ 
    public StuffModel? Stuff { get; set; }
}