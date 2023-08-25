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

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        await AuthorizedSendAsync<StuffLocationModel>(new StuffLocationRequest { LocationId = location.Id, StuffId = stuffA.Id, Count = 2 }, HttpMethod.Post, ApiRoutes.StuffLocations);

        var inventory = await AuthorizedSendAsync(new EventRequest { Type = EventType.Inventory, StuffId = stuffA.Id }, HttpMethod.Post, ApiRoutes.Events);
        Assert.True(inventory.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedGetAsync<PagedList<EventListItem>>($"{ApiRoutes.EventsByStuff}/{stuffA.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.NotEmpty(result.List);

        var purchaseEvent = result.List.FirstOrDefault(x => x.Type == EventType.Purchased);
        Assert.NotNull(purchaseEvent);

        var purchaseData = purchaseEvent.Data;
        Assert.NotNull(purchaseData);
        Assert.Equal(stuffA.Id, purchaseData.Stuff?.Id);
        Assert.Equal(supplier.Id, purchaseData.Supplier?.Id);
        Assert.Equal(purchaseItem.Quantity, purchaseData.PurchaseItem?.Quantity);

        var movedEvent = result.List.FirstOrDefault(x => x.Type == EventType.Moved);
        Assert.NotNull(movedEvent);

        var movedData = movedEvent.Data;
        Assert.NotNull(movedData);
        Assert.NotNull(movedData.ToLocation);
        Assert.Equal(location.Id, movedData.ToLocation.Id);

        var inventoryEvent = result.List.FirstOrDefault(x => x.Type == EventType.Inventory);
        Assert.NotNull(inventoryEvent);

        var inventoryData = inventoryEvent.Data;
        Assert.NotNull(inventoryData);
        Assert.NotNull(inventoryData.Locations);
        Assert.Single(inventoryData.Locations);

        var single = inventoryData.Locations.Single().Location;
        Assert.Equal(location.Id, single.Id);
    }
}