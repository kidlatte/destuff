namespace Destuff.Tests.PurchaseItems;

public class PurchaseItemsDeleteRequestShould : IntegrationTestBase
{
    public PurchaseItemsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.PurchaseItems)
    {
    }

    [Fact]
    public async Task Delete_PurchaseItem()
    {
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var create = new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 };
        var created = await AuthorizedSendAsync<PurchaseItemModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.PurchaseItems}/{created.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.PurchaseItems}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_PurchaseItem()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var create = new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 };
        var model = await AuthorizedSendAsync<PurchaseItemModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.PurchaseItems}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}