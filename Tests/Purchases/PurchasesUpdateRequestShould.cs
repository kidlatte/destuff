namespace Destuff.Tests.Purchases;

public class PurchasesUpdateRequestShould : IntegrationTestBase
{
    public PurchasesUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Update_Purchase()
    {
        // Arrange
        var create = new PurchaseRequest { Receipt = new DateTime(2000, 1, 1) };
        var model = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new PurchaseRequest { Receipt = new DateTime(2020, 1, 1) };
        var result = await AuthorizedPutAsync<PurchaseModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Receipt, result?.Receipt);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Purchase()
    {
        // Arrange
        var create = new PurchaseRequest();
        var model = await AuthorizedSendAsync<PurchaseModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new PurchaseRequest();
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Purchases}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}