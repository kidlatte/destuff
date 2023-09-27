namespace Destuff.Shared.Models;

public class DashboardModel
{
    public int StuffCount { get; set; }
    public int LocationCount { get; set; }
    public int InventoriedInMonth { get; set; }

    public required ICollection<StuffBasicModel> LatestStuffs { get; set; }
}
