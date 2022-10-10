using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsCreateRequestShould : IntegrationTestBase
{
    public StuffsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Create_Stuff()
    {
        // Arrange
        var model = new StuffCreateModel { Name = "Stuff 001" };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }


    [Theory]
    [InlineData("FooBar", "foobar")]
    [InlineData("Foo Bar", "foo-bar")]
    [InlineData("Foo-Baz", "foo-baz")]
    [InlineData("F00Bar", "f00bar")]
    public async Task Create_Stuff_Slugs(string name, string slug)
    {
        // Arrange
        var create = new StuffCreateModel { Name = name };

        // Act
        var result = await AuthorizedSendAsync<StuffModel>(create);

        // Assert
        Assert.Equal(slug, result?.Slug);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Stuff()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Existing Slug" };
        var created = await AuthorizedSendAsync(create);
        Assert.True(created?.IsSuccessStatusCode);

        // Act
        var sameSlug = new StuffCreateModel { Name = "existing - slug" };
        var result = await AuthorizedSendAsync(sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Create_Stuff()
    {
        // Arrange
        var model = new StuffCreateModel();

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Stuff()
    {
        // Arrange
        var model = new StuffCreateModel { Name = "Unauthorized" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}