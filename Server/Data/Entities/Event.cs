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
    Stuff? Stuff { get; set; }
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
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public EventType Type { get; set; }
    public string? Summary { get; set; }
    public EventData? Data { get; set; }

    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }
   
    public ICollection<Upload>? Uploads { get; set; }
}

public class EventData
{ 
    public StuffModel? Stuff { get; set; }
    public LocationListItem? Location { get; set; }
    public LocationListItem? ToLocation { get; set; }
}