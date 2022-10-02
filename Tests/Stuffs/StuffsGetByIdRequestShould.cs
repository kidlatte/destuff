using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsGetByIdRequestShould : IntegrationTestBase
{
    public StuffsGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Get_Stuff_Details()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "New Location" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<StuffModel>($"{ApiRoutes.Stuffs}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task Fail_Nonexistent_Stuff_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.Stuffs}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}