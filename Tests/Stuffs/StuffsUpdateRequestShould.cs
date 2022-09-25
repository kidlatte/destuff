using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsUpdateRequestShould : IntegrationTestBase
{
    public StuffsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Update_Stuff()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffCreateModel { Name = "Updated Stuff" };
        var result = await AuthorizedPutAsync<StuffModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result?.Name);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Stuff()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Existing Slug" };
        var created = await AuthorizedSendAsync(create, HttpMethod.Post);
        Assert.True(created?.IsSuccessStatusCode);

        var newSlug = new StuffCreateModel { Name = "New Slug" };
        var model = await AuthorizedSendAsync<StuffModel>(newSlug, HttpMethod.Post);
        Assert.NotNull(model?.Id);

        // Act
        var sameSlug = new StuffCreateModel { Name = "existing - slug" };
        var result = await AuthorizedPutAsync(model?.Id!, sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Update_Stuff()
    {
       // Arrange
        var create = new StuffCreateModel { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffCreateModel { Name = null };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Stuff()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffCreateModel { Name = "Updated Stuff" };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Stuffs}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}