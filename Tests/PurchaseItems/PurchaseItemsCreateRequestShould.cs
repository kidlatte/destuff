namespace Destuff.Tests.PurchaseItems;

public class PurchaseItemsCreateRequestShould : IntegrationTestBase
{
    public PurchaseItemsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.PurchaseItems)
    {
    }

    [Fact]
    public async Task Create_PurchaseItem()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var model = new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Null_Stuff_Create_PurchaseItem()
    {
        // Arrange
        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var model = new PurchaseItemRequest { PurchaseId = purchase.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_PurchaseItem()
    {
        // Arrange
        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var model = new PurchaseItemRequest { PurchaseId = purchase.Id };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}