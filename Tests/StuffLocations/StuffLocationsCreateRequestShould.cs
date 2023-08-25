using Destuff.Server.Data.Entities;

namespace Destuff.Tests.StuffLocations;

public class StuffLocationsCreateRequestShould : IntegrationTestBase
{
    public StuffLocationsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.StuffLocations)
    {
    }

    [Fact]
    public async Task Create_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "New Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var model = new StuffLocationRequest { StuffId = stuff.Id, LocationId = location.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Return_StuffAndLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "New Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var model = new StuffLocationRequest { StuffId = stuff.Id, LocationId = location.Id };

        // Act
        var result = await AuthorizedSendAsync<StuffLocationModel>(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stuff.Id, result.Stuff.Id);
        Assert.Equal(location.Id, result.Location.Id);
    }

    [Fact]
    public async Task Fail_Null_Location_Create_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var model = new StuffLocationRequest { StuffId = stuff.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_StuffLocation()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Loaction" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var model = new StuffLocationRequest { StuffId = stuff.Id, LocationId = location.Id };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}