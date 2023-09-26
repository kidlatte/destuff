using Destuff.Server.Data.Entities;
using System.Collections.Generic;

namespace Destuff.Tests.Locations;

public class LocationsOrderUpRequestShould : IntegrationTestBase
{
    public LocationsOrderUpRequestShould() : base(HttpMethod.Put, ApiRoutes.LocationOrderUp)
    {
    }

    [Fact]
    public async Task MoveOrderUp_Location()
    {
        // Arrange
        var locationA = await AuthorizedSendAsync<LocationModel>(new { Name = "Location A" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationA);

        var locationB = await AuthorizedSendAsync<LocationModel>(new { Name = "Location B" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationB);

        // Act
        await AuthorizedPutAsync(locationB.Id);
        var result = await AuthorizedGetAsync<List<LocationTreeItem>>(ApiRoutes.Locations);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var last = result.Last();
        Assert.Equal(locationA.Id, last.Id);
    }

    [Fact]
    public async Task Fail_Unauthorized_MoveOrderUp_Location()
    {
        // Arrange
        var create = new LocationRequest { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Put, $"{ApiRoutes.LocationOrderUp}/{model.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}