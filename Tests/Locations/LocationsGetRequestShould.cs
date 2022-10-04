using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Locations;

public class LocationsGetRequestShould : IntegrationTestBase
{
    public LocationsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Get_Locations_List()
    {
        // Arrange
        var model = new LocationCreateModel { Name = "Location 01" };
        await AuthorizedSendAsync<LocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(model.Name, result.Last().Name);
    }

    [Fact]
    public async Task Get_Locations_Nested()
    {
        // Arrange
        var parentCreate = new LocationCreateModel { Name = "Parent 01" };
        var parent = await AuthorizedSendAsync<LocationModel>(parentCreate, HttpMethod.Post);
        var model = new LocationCreateModel { Name = "Child 01", ParentId = parent?.Id };
        await AuthorizedSendAsync<LocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        
        var children = result.Last().Children;
        Assert.NotNull(children);
        Assert.NotEmpty(children);
    }

    [Fact]
    public async Task Get_Locations_Three_Layers()
    {
        // Arrange
        var create01 = new LocationCreateModel { Name = "Layer 01" };
        var layer01 = await AuthorizedSendAsync<LocationModel>(create01, HttpMethod.Post);
        var create02 = new LocationCreateModel { Name = "Layer 02", ParentId = layer01?.Id };
        var layer02 = await AuthorizedSendAsync<LocationModel>(create02, HttpMethod.Post);
        var create03 = new LocationCreateModel { Name = "Layer 03", ParentId = layer02?.Id };
        await AuthorizedSendAsync(create03, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<List<LocationModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var children = result.Last().Children;
        Assert.NotNull(children);
        Assert.NotEmpty(children);

        var grandchildren = result?.Last().Children?.Last().Children;
        Assert.NotNull(grandchildren);
        Assert.NotEmpty(grandchildren);
    }
}