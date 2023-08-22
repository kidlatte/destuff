namespace Destuff.Tests.Events;

public class EventsGetByStuffRequestShould : IntegrationTestBase
{
    public EventsGetByStuffRequestShould() : base(HttpMethod.Get, ApiRoutes.EventsByStuff)
    {
    }

    [Fact]
    public async Task Get_Events()
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff A" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff B" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var inventory = await AuthorizedSendAsync(new InventoryRequest { StuffId = stuffA.Id }, HttpMethod.Post, ApiRoutes.Inventories);
        Assert.True(inventory.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedGetAsync<PagedList<EventListItem>>($"{ApiRoutes.EventsByStuff}/{stuffA.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }
}