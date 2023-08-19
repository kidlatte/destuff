namespace Destuff.Tests.StuffLocations;

public class StuffLocationsDeleteRequestShould : IntegrationTestBase
{
    public StuffLocationsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.StuffLocations)
    {
    }

    [Fact]
    public async Task Delete_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Loaction" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var createResponse = await AuthorizedSendAsync(new StuffLocationRequest { LocationId = location.Id, StuffId = stuff.Id }, HttpMethod.Post);
        Assert.True(createResponse.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedSendAsync(null, HttpMethod.Delete, $"{ApiRoutes.StuffLocations}/{stuff.Id}/{location.Id}");

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Loaction" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var createResponse = await AuthorizedSendAsync(new StuffLocationRequest { LocationId = location.Id, StuffId = stuff.Id }, HttpMethod.Post);
        Assert.True(createResponse.IsSuccessStatusCode);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.StuffLocations}/{stuff.Id}/{location.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}