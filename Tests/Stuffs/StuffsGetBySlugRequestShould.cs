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

public class StuffsGetBySlugRequestShould : IntegrationTestBase
{
    public StuffsGetBySlugRequestShould() : base(HttpMethod.Get, ApiRoutes.Stuffs) 
    {
    }

    [Fact]
    public async Task Get_Stuff_By_Slug()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Stuff Slug" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post, ApiRoutes.Stuffs);

        // Act
        var result = await AuthorizedGetAsync<StuffModel>($"{ApiRoutes.StuffSlug}/stuff-slug");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(model?.Id, result?.Id);
    }
    
    [Fact]
    public async Task Fail_Nonexistent_Stuff_By_Slug()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.StuffSlug}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Stuff_By_Slug()
    {
        // Arrange
        var create = new StuffCreateModel { Name = "Unauthorized Slug" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post, ApiRoutes.Stuffs);

        // Act
        var result = await SendAsync(null, HttpMethod.Get, $"{ApiRoutes.StuffSlug}/unauthorized-slug");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}