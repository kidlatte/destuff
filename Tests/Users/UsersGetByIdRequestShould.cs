namespace Destuff.Tests.Users;

public class UsersGetByIdRequestShould : IntegrationTestBase
{
    public UsersGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.UserSettings)
    {
    }

    [Fact]
    public async Task Get_User_Settings()
    {
        // Act
        var result = await AuthorizedGetAsync<UserSettings>();

        // Assert
        Assert.NotNull(result);
        Assert.False(result?.InventoryEnabled);
        Assert.False(result?.PurchasesEnabled);
    }
}