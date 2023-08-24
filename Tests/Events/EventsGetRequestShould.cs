namespace Destuff.Tests.Events;

public class EventsGetRequestShould : IntegrationTestBase
{
    public EventsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Events)
    {
    }

    [Fact]
    public async Task Get_Events()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var inventory = await AuthorizedSendAsync(new EventRequest { Type = EventType.Inventory, StuffId = stuff.Id }, HttpMethod.Post, ApiRoutes.Events);
        Assert.True(inventory.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedSendAsync<PagedList<EventListItem>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }
}