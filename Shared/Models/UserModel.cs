namespace Destuff.Shared.Models;

public class UserModel
{
    public required string UserName { get; set; }
}

public class UserSettings
{
    public bool InventoryEnabled { get; set; }
    public bool PurchasesEnabled { get; set; }
}