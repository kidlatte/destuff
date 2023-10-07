namespace Destuff.Client.Services;

public interface IEventsManager
{
    event EventHandler<bool>? InventoryEnabledChanged;
    event EventHandler<bool>? PurchasesEnabledChanged;

    void InvokeInventoryEnabledChanged(bool value);
    void InvokePurchasesEnabledChanged(bool value);
}

public class EventsManager : IEventsManager
{
    public event EventHandler<bool>? InventoryEnabledChanged;
    public event EventHandler<bool>? PurchasesEnabledChanged;

    public void InvokeInventoryEnabledChanged(bool value) => 
        InventoryEnabledChanged?.Invoke(this, value);

    public void InvokePurchasesEnabledChanged(bool value) => 
        PurchasesEnabledChanged?.Invoke(this, value);
}
