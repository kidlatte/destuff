namespace Destuff.Tests.Users;

public class UsersUpdateRequestShould : IntegrationTestBase
{
    public UsersUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.UserSettings)
    {
    }

    [Fact]
    public async Task Update_User()
    {
        // Arrange
        var update = new UserSettings { InventoryEnabled = true, PurchasesEnabled = true };

        // Act
        var result = await AuthorizedSendAsync<UserSettings>(update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.InventoryEnabled, result?.InventoryEnabled);
        Assert.Equal(update.PurchasesEnabled, result?.PurchasesEnabled);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_User()
    {
        // Arrange
        var update = new UserSettings { InventoryEnabled = true, PurchasesEnabled = true };

        // Act
        var result = await SendAsync(update, HttpMethod.Put, ApiRoutes.UserSettings);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}