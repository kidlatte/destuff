using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Locations;

public class LocationsDeleteRequestShould : IntegrationTestBase
{
    public LocationsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Delete_Location()
    {
        var create = new LocationCreateModel { Name = "Created Location" };
        var created = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.Locations}/{created?.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created?.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.Locations}/{created?.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_Location()
    {
        // Arrange
        var create = new LocationCreateModel { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Locations}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}