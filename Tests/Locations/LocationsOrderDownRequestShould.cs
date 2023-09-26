using Destuff.Server.Data.Entities;
using System.Collections.Generic;

namespace Destuff.Tests.Locations;

public class LocationsOrderDownRequestShould : IntegrationTestBase
{
    public LocationsOrderDownRequestShould() : base(HttpMethod.Put, ApiRoutes.LocationOrderDown)
    {
    }

    [Fact]
    public async Task MoveOrderDown_Location()
    {
        // Arrange
        var locationA = await AuthorizedPostAsync<LocationModel>(new { Name = "Location A" }, ApiRoutes.Locations);
        Assert.NotNull(locationA);

        var locationB = await AuthorizedPostAsync<LocationModel>(new { Name = "Location B" }, ApiRoutes.Locations);
        Assert.NotNull(locationB);

        // Act
        await AuthorizedPutAsync(locationA.Id);
        var result = await AuthorizedGetAsync<List<LocationTreeItem>>(ApiRoutes.Locations);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var last = result.Last();
        Assert.Equal(locationA.Id, last.Id);
    }

    [Fact]
    public async Task Fail_Unauthorized_MoveOrderDown_Location()
    {
        // Arrange
        var create = new LocationRequest { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Put, $"{ApiRoutes.LocationOrderDown}/{model.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}