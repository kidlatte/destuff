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

public class LocationsDetailsRequestShould : IntegrationTestBase
{
    public LocationsDetailsRequestShould() : base(HttpMethod.Get, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Get_Location_Details()
    {
        // Arrange
        var create = new LocationRequest { Name = "New Loaction" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<LocationModel>($"{ApiRoutes.Locations}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task Fail_Nonexistent_Location_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.Locations}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}