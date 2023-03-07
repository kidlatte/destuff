namespace Destuff.Tests.PurchaseItems;

public class PurchaseItemsGetByIdRequestShould : IntegrationTestBase
{
    public PurchaseItemsGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.PurchaseItems)
    {
    }

    [Fact]
    public async Task Get_PurchaseItem_Details()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var create = new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 };
        var model = await AuthorizedSendAsync<PurchaseItemModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<PurchaseItemModel>($"{ApiRoutes.PurchaseItems}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task Fail_Nonexistent_PurchaseItem_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.PurchaseItems}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}