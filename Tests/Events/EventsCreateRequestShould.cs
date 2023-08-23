namespace Destuff.Tests.Events;

public class EventsCreateRequestShould : IntegrationTestBase
{
    public EventsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Events)
    {
    }

    [Fact]
    public async Task Create_Event()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var model = new EventRequest { StuffId = stuff.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Null_Stuff_Create_Event()
    {
        // Arrange
        var model = new EventRequest { Count = 1 };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Event()
    {
        // Arrange
        var model = new EventRequest { Count = 1 };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}