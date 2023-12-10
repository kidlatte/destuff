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

}
