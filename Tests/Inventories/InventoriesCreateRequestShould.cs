namespace Destuff.Tests.Inventories;

public class InventoriesCreateRequestShould : IntegrationTestBase
{
    public InventoriesCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Inventories)
    {
    }

    [Fact]
    public async Task Create_Inventory()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var model = new InventoryRequest { StuffId = stuff.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }
}