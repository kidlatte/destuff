using Destuff.Server.Data.Entities;

namespace Destuff.Tests.Stuffs;

public class StuffsScrapeUrlRequestShould : IntegrationTestBase
{
    public StuffsScrapeUrlRequestShould() : base(HttpMethod.Put, ApiRoutes.StuffScrapeUrl)
    {
    }

    [Fact]
    public async Task ScrapeUrl_Stuff()
    {
        // Arrange
        var create = new StuffRequest { Name = "Created Stuff", Url = "https://ogp.me/" };
        var model = await AuthorizedPostAsync<StuffModel>(create, ApiRoutes.Stuffs);
        Assert.NotNull(model);

        // Act
        var result = await AuthorizedPutAsync<StuffOpenGraph>(model.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Fail_Unauthorized_ScrapeUrl_Stuff()
    {
        // Arrange
        var create = new StuffRequest { Name = "Created Stuff", Url = "https://ogp.me/" };
        var model = await AuthorizedPostAsync<StuffModel>(create, ApiRoutes.Stuffs);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Put, $"{ApiRoutes.StuffScrapeUrl}/{model.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}