using Destuff.Shared.Models;

namespace Destuff.Server.Data.Entities;

public class Maintenance : Entity
{
    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }
    public ICollection<MaintenanceLog>? Logs { get; set; }

    public required string Name { get; set; }
    public int EveryXDays { get; set; }
    public DateTime Next { get; set; }
    public string? Notes { get; set; }
    public MaintenanceData? Data { get; set; }
}

public class MaintenanceData
{
    public DateTime? LastCompleted { get; set; }
}

public class MaintenanceLog : Entity, IEvent
{
    public required string Name { get; set; }
    public string? Notes { get; set; }
    public EventType Type => EventType.Maintained;

    public int? MaintenanceId { get; set; }
    public Maintenance? Maintenance { get; set; }

    public int StuffId { get; set; }
    public Stuff? Stuff { get; set; }

    // [Required] TODO: remove in next migration
    public DateTime? DateTime { get; set; }
    public int Count { get; set; }
    public string? Summary { get; set; }
    public EventData? Data { get; set; }
}
