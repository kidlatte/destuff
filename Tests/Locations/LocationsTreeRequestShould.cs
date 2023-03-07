using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Locations;

public class LocationsTreeRequestShould : IntegrationTestBase
{
    public LocationsTreeRequestShould() : base(HttpMethod.Get, ApiRoutes.LocationTree)
    {
    }

    [Fact]
    public async Task Get_Location_Tree()
    {
        // Arrange
        var create01 = new LocationRequest { Name = "Layer01" };
        var layer01 = await AuthorizedSendAsync<LocationModel>(create01, HttpMethod.Post, ApiRoutes.Locations);
        var create02 = new LocationRequest { Name = "Layer02", ParentId = layer01?.Id };
        var layer02 = await AuthorizedSendAsync<LocationModel>(create02, HttpMethod.Post, ApiRoutes.Locations);
        var create03 = new LocationRequest { Name = "Layer03", ParentId = layer02?.Id };
        var layer03 = await AuthorizedSendAsync<LocationModel>(create03, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(layer03?.Id);

        // Act
        var result = await AuthorizedGetAsync<LocationModel>($"{ApiRoutes.LocationTree}/{layer01?.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Id);
        Assert.NotNull(result.Children);
        Assert.NotEmpty(result.Children);

        var children = result.Children.First().Children;
        Assert.NotNull(children);
        Assert.NotEmpty(children);
        Assert.Equal(layer03.Id, children.First().Id);
    }

    [Fact]
    public async Task Fail_Unauthorized_Location_Tree()
    {
        // Arrange
        var create = new LocationRequest { Name = "Unauthorized Tree" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post, ApiRoutes.Locations);

        // Act
        var result = await SendAsync(null, HttpMethod.Get, $"{ApiRoutes.LocationTree}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}