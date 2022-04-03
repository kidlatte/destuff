using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Auth;

public class LocationsIndexRequestShould : IntegrationTestBase
{
    public LocationsIndexRequestShould() : base(HttpMethod.Get, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Succeed_Get_Locations()
    {
        // Arrange
        var model = new LocationCreateModel { Name = "Location01" };
        await AuthorizedSendAsync<LocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.True(result?.Count > 0);
        Assert.Equal(model.Name, result?.First().Name);
    }
}