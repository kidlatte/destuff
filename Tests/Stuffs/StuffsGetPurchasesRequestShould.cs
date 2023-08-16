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

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItem = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItem);

        // Act
        var result = await AuthorizedGetAsync<IEnumerable<PurchaseModel>>($"{ApiRoutes.StuffPurchases}/{stuff.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(purchase.Id, result.First().Id);
    }
}