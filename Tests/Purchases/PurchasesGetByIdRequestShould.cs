namespace Destuff.Tests.Purchases;

public class PurchasesGetByIdRequestShould : IntegrationTestBase
{
    public PurchasesGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Get_Purchase_Details()
    {
        // Arrange
        var create = new PurchaseRequest();
        var model = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<PurchaseModel>($"{ApiRoutes.Purchases}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task Get_Purchase_ItemCount()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier", Name = "Supplier 001" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { SupplierId = supplier.Id }, HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItem = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Quantity = 2, Price = 10 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItem);

        // Act
        var result = await AuthorizedGetAsync<PurchaseModel>($"{ApiRoutes.Purchases}/{purchase.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ItemCount);
        Assert.Equal(20, result.Price);
    }

    [Fact]
    public async Task Fail_Nonexistent_Purchase_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.Purchases}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}