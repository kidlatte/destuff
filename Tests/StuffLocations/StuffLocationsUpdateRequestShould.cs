namespace Destuff.Tests.StuffLocations;

public class StuffLocationsUpdateRequestShould : IntegrationTestBase
{
    public StuffLocationsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.StuffLocations)
    {
    }

    [Fact]
    public async Task Update_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "New Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var create = new StuffLocationRequest { Count = 1, LocationId = location.Id, StuffId = stuff.Id };
        var created = await AuthorizedSendAsync<StuffLocationModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        // Act
        var update = new StuffLocationRequest { Count = 2, LocationId = location.Id, StuffId = stuff.Id };
        var result = await AuthorizedSendAsync<StuffLocationModel>(update, HttpMethod.Put);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Count, result.Count);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_StuffLocation()
    {
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "New Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var create = new StuffLocationRequest { Count = 1, LocationId = location.Id, StuffId = stuff.Id };
        var created = await AuthorizedSendAsync<StuffLocationModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        // Act
        var update = new StuffLocationRequest { Count = 2, LocationId = location.Id, StuffId = stuff.Id };
        var result = await SendAsync(update, HttpMethod.Put);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}