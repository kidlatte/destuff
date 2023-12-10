using Destuff.Shared.Models;

namespace Destuff.Server.Data.Entities;

public class MaintenanceLog : Entity, IEvent
{
    public required string Name { get; set; }
    public string? Notes { get; set; }
    public EventType Type => EventType.Maintained;

    public int? MaintenanceId { get; set; }
    public Maintenance? Maintenance { get; set; }

    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }

    public int Count { get; set; }
    public DateTime? DateTime { get; set; }
    public string? Summary { get; set; }
    public EventData? Data { get; set; }
}
