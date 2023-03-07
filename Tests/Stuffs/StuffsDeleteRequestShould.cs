using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsDeleteRequestShould : IntegrationTestBase
{
    public StuffsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Delete_Stuff()
    {
        var create = new StuffRequest { Name = "Created Stuff" };
        var created = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.Stuffs}/{created?.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created?.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.Stuffs}/{created?.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_Stuff()
    {
        // Arrange
        var create = new StuffRequest { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Stuffs}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}