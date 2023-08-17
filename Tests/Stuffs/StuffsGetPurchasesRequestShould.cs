using System.Collections.Generic;

namespace Destuff.Tests.Stuffs;

public class StuffsGetPurchasesRequestShould : IntegrationTestBase
{
    public StuffsGetPurchasesRequestShould() : base(HttpMethod.Get, ApiRoutes.StuffPurchases)
    {
    }

    [Fact]
    public async Task Get_Stuff_Details()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier", Name = "Supplier 001" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { SupplierId = supplier.Id }, HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItem = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItem);

        // Act
        var results = await AuthorizedGetAsync<IEnumerable<PurchaseItemSupplier>>($"{ApiRoutes.StuffPurchases}/{stuff.Id}");

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);

        var result = results.First();
        Assert.Equal(purchaseItem.Id, result.Id);
        Assert.Equal(purchase.Id, result.Purchase.Id);
        Assert.Equal(supplier.Id, result.Purchase.Supplier?.Id);
    }
}