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
        var create = new PurchaseCreateModel();
        var model = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<PurchaseModel>($"{ApiRoutes.Purchases}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
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