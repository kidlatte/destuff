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

public class LocationsSlugRequestShould : IntegrationTestBase
{
    public LocationsSlugRequestShould() : base(HttpMethod.Get, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Get_Location_By_Slug()
    {
        // Arrange
        var create = new LocationCreateModel { Name = "Location Slug" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<LocationModel>($"{ApiRoutes.Locations}/s/location-slug");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(model?.Id, result?.Id);
    }
    
    [Fact]
    public async Task Fail_Nonexistent_Location_By_Slug()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.Locations}/s/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

}