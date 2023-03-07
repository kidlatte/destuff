using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Locations;

public class LocationsCreateRequestShould : IntegrationTestBase
{
    public LocationsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Locations)
    {
    }

    [Fact]
    public async Task Create_Root_Location()
    {
        // Arrange
        var model = new LocationRequest { Name = "Root Location" };

        // Act
        var result = await AuthorizedSendAsync<LocationModel>(model);

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Null(result?.ParentId);
    }

    [Fact]
    public async Task Create_Child_Location()
    {
        // Arrange
        var parentCreate = new LocationRequest { Name = "Parent01" };
        var parent = await AuthorizedSendAsync<LocationModel>(parentCreate);

        // Act
        var model = new LocationRequest { Name = "Child01", ParentId = parent?.Id };
        var result = await AuthorizedSendAsync<LocationModel>(model);

        // Assert
        Assert.NotNull(result?.ParentId);
        Assert.Equal(parent?.Id, result?.ParentId);
    }


    [Theory]
    [InlineData("FooBar", "foobar")]
    [InlineData("Foo Bar", "foo-bar")]
    [InlineData("Foo-Baz", "foo-baz")]
    [InlineData("F00Bar", "f00bar")]
    public async Task Create_Location_Slugs(string name, string slug)
    {
        // Arrange
        var create = new LocationRequest { Name = name };

        // Act
        var result = await AuthorizedSendAsync<LocationModel>(create);

        // Assert
        Assert.Equal(slug, result?.Slug);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Location()
    {
        // Arrange
        var create = new LocationRequest { Name = "Existing Slug" };
        var created = await AuthorizedSendAsync(create);
        Assert.True(created?.IsSuccessStatusCode);

        // Act
        var sameSlug = new LocationRequest { Name = "existing - slug" };
        var result = await AuthorizedSendAsync(sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Create_Location()
    {
        // Arrange
        var model = new LocationRequest();

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Location()
    {
        // Arrange
        var model = new LocationRequest { Name = "Unauthorized" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}