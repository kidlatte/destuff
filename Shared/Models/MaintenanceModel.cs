namespace Destuff.Shared.Models;

public class MaintenanceRequest : IRequest
{
    public string? StuffId { get; set; }
    public string? Name { get; set; }
    public int EveryXDays { get; set; }
    public string? Notes { get; set; }
}

public class MaintenanceListItem : IModel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public int EveryXDays { get; set; }
    public DateTime Next { get; set; }
}

public class MaintenanceModel: MaintenanceListItem
{
    public StuffBasicModel? Stuff { get; set; }
    public string? Notes { get; set; }
}
