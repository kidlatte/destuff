using Destuff.Shared.Models;

namespace Destuff.Server.Data.Entities;

public interface IEvent
{
    EventType Type { get; }

    DateTime DateTime { get; set; }
    int Count { get; set; }
    string? Summary { get; set; }
    string? Notes { get; set; }

    int StuffId { get; set; }
    Stuff? Stuff { get; set; }
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
    public StuffBasicModel? Stuff { get; set; }
    public LocationListItem? FromLocation { get; set; }
    public LocationListItem? Location { get; set; }
    public SupplierBasicModel? Supplier { get; set; }
}