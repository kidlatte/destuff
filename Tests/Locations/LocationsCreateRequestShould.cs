using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Auth;

public class LocationsCreateRequestShould : IntegrationTestBase
{
    public LocationsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Create_New_Location()
    {
        // Arrange
        var model = new LocationCreateModel { Name = "Location01" };

        // Act
        var result = await AuthorizedSendAsync<LocationModel>(model);

        // Assert
        Assert.NotNull(result?.Id);
    }

    [Fact]
    public async Task Fail_Unauthorized_Request()
    {
        // Arrange
        var model = new LocationCreateModel { Name = "Location01" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name()
    {
        // Arrange
        var model = new LocationCreateModel();

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

}