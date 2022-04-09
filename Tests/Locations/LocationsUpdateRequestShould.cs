using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Locations;

public class LocationsUpdateRequestShould : IntegrationTestBase
{
    public LocationsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Update_Location()
    {
        // Arrange
        var create = new LocationCreateModel { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new LocationCreateModel { Name = "Updated Location", ParentId = model?.ParentId };
        var result = await AuthorizedPutAsync<LocationModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result?.Name);
        Assert.Equal(model?.ParentId, result?.ParentId);
    }

    [Fact]
    public async Task Fail_Null_Name_Update_Location()
    {
       // Arrange
        var create = new LocationCreateModel { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new LocationCreateModel { Name = null, ParentId = model?.ParentId };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Location()
    {
        // Arrange
        var create = new LocationCreateModel { Name = "Created Location" };
        var model = await AuthorizedSendAsync<LocationModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new LocationCreateModel { Name = "Updated Location", ParentId = model?.ParentId };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Locations}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}