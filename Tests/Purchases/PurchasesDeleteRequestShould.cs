namespace Destuff.Tests.Purchases;

public class PurchasesDeleteRequestShould : IntegrationTestBase
{
    public PurchasesDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Delete_Purchase()
    {
        var create = new PurchaseRequest();
        var created = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.Purchases}/{created.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.Purchases}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_Purchase()
    {
        // Arrange
        var create = new PurchaseRequest();
        var model = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Purchases}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}