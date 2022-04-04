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
    public async Task Get_Locations_List()
    {
        // Arrange
        var model = new LocationCreateModel { Name = "Location01" };
        await AuthorizedSendAsync<LocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(model.Name, result?.First().Name);
    }

    [Fact]
    public async Task Get_Locations_Nested()
    {
        // Arrange
        var parentCreate = new LocationCreateModel { Name = "Parent01" };
        var parent = await AuthorizedSendAsync<LocationModel>(parentCreate, HttpMethod.Post);
        var model = new LocationCreateModel { Name = "Child01", ParentId = parent?.Id };
        await AuthorizedSendAsync<LocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotEmpty(result);
        Assert.NotEmpty(result?.First().Children);
    }
}