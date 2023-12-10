namespace Destuff.Shared.Models;

public class MaintenanceLogRequest : IRequest
{
    public string? MaintenanceId { get; set; }
    public string? StuffId { get; set; }
    public string? Name { get; set; }
    public DateTime? DateTime { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceLogListItem : IModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTime? DateTime { get; set; }
    public MaintenanceListItem? Maintenance { get; set; }
    public StuffBasicModel? Stuff { get; set; }
}

public class MaintenanceLogModel : MaintenanceLogListItem
{
    public string? Notes { get; set; }
}
