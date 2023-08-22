namespace Destuff.Tests.Events;

public class EventsGetByStuffRequestShould : IntegrationTestBase
{
    public EventsGetByStuffRequestShould() : base(HttpMethod.Get, ApiRoutes.EventsByStuff)
    {
    }

    [Fact]
    public async Task Get_Events()
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff A" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff B" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier", Name = "Supplier 001" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { SupplierId = supplier.Id }, HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItem = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuffA.Id, Price = 1 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItem);

        var inventory = await AuthorizedSendAsync(new InventoryRequest { StuffId = stuffA.Id }, HttpMethod.Post, ApiRoutes.Inventories);
        Assert.True(inventory.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedGetAsync<PagedList<EventListItem>>($"{ApiRoutes.EventsByStuff}/{stuffA.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.NotEmpty(result.List);

        var eventPurchase = result.List.FirstOrDefault(x => x.Type == EventType.Purchase);
        Assert.NotNull(eventPurchase);

        var purchaseData = eventPurchase.Data;
        Assert.NotNull(purchaseData);
        Assert.Equal(stuffA.Id, purchaseData.Stuff?.Id);
        Assert.Equal(supplier.Id, purchaseData.Supplier?.Id);
        Assert.Equal(purchaseItem.Quantity, purchaseData.PurchaseItem?.Quantity);

        var eventInventory = result.List.FirstOrDefault(x => x.Type == EventType.Inventory);
        Assert.NotNull(eventInventory);
    }
}