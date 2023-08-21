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

        var locationA = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "Location A" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationA);

        var locationB = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "Location B" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationB);

        var create = new StuffLocationRequest { Count = 1, LocationId = locationA.Id, StuffId = stuff.Id };
        var created = await AuthorizedSendAsync<StuffLocationModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        // Act 01
        var update01 = new StuffLocationRequest { Count = 2, LocationId = locationA.Id, StuffId = stuff.Id };
        var updated01 = await AuthorizedSendAsync<StuffLocationModel>(update01, HttpMethod.Put, $"{ApiRoutes.StuffLocations}/{created.Stuff.Id}/{created.Location.Id}");

        // Assert 01
        Assert.NotNull(updated01);
        Assert.Equal(update01.Count, updated01.Count);
        Assert.Equal(update01.LocationId, updated01.Location.Id);

        // Act 02
        var update02 = new StuffLocationRequest { Count = 2, LocationId = locationB.Id, StuffId = stuff.Id };
        var updated02 = await AuthorizedSendAsync<StuffLocationModel>(update02, HttpMethod.Put, $"{ApiRoutes.StuffLocations}/{created.Stuff.Id}/{created.Location.Id}");

        // Assert 02
        Assert.NotNull(updated02);
        Assert.Equal(update02.Count, updated02.Count);
        Assert.Equal(update02.LocationId, updated02.Location.Id);
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
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.StuffLocations}/{stuff.Id}/{location.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}