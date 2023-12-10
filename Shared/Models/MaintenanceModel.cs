using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class MaintenanceRequest : IRequest
{
    [Required]
    public string? StuffId { get; set; }

    [Required]
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
    public required StuffBasicModel Stuff { get; set; }
    public string? Notes { get; set; }
}
