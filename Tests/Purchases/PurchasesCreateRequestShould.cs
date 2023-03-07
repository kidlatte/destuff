namespace Destuff.Tests.Purchases;

public class PurchasesCreateRequestShould : IntegrationTestBase
{
    public PurchasesCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Create_Purchase()
    {
        // Arrange
        var model = new PurchaseRequest();

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Purchase()
    {
        // Arrange
        var model = new PurchaseRequest();

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}