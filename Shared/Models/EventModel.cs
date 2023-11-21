using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class EventRequest : IRequest
{
    public EventType Type { get; set; }
    public int Count { get; set; }
    public DateTime? DateTime { get; set; }
    public string? Recipient { get; set; }
    public string? Notes { get; set; }

    [Required]
    public string? StuffId { get; set; }
}

public class EventModel : IModel
{
    public required string Id { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public string? Summary { get; set; }

    public required StuffModel Stuff { get; set; }
}

public class EventListItem : IModel
{
    public required string Id { get; set; }
    public EventType Type { get; set; }
    public int Count { get; set; }
    public string? Notes { get; set; }

    public DateTime DateTime { get; set; }
    public string? Summary { get; set; }
    public EventData? Data { get; set; }
}

public class EventData
{
    public int? Difference { get; set; }
    public string? Recipient { get; set; }
    public PurchaseItemBasicModel? PurchaseItem { get; set; }
    public StuffBasicModel? Stuff { get; set; }
    public LocationListItem? FromLocation { get; set; }
    public LocationListItem? ToLocation { get; set; }
    public ICollection<StuffLocationListItem>? Locations { get; set; }
    public SupplierBasicModel? Supplier { get; set; }
}

public enum EventType
{
    Inventory,
    Purchased,
    Moved,
    Lent,
    Donated,
    Consumed,
    Disposed,
    Missing
}
