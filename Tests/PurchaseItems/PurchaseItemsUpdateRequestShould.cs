namespace Destuff.Tests.PurchaseItems;

public class PurchaseItemsUpdateRequestShould : IntegrationTestBase
{
    public PurchaseItemsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.PurchaseItems)
    {
    }

    [Fact]
    public async Task Update_PurchaseItem()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var create = new PurchaseItemRequest { Price = 1, PurchaseId = purchase.Id, StuffId = stuff.Id };
        var model = await AuthorizedSendAsync<PurchaseItemModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new PurchaseItemRequest { Price = 2, PurchaseId = purchase.Id, StuffId = stuff.Id };
        var result = await AuthorizedPutAsync<PurchaseItemModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Price, result?.Price);
    }

    [Fact]
    public async Task Fail_Null_Purchase_Stuff_Update_PurchaseItem()
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
        var update = new PurchaseItemRequest { PurchaseId = purchase.Id };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_PurchaseItem()
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
        var update = new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 2 };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.PurchaseItems}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}