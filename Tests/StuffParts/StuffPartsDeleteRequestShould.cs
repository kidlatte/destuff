namespace Destuff.Tests.StuffParts;

public class StuffPartsDeleteRequestShould : IntegrationTestBase
{
    public StuffPartsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.StuffParts)
    {
    }

    [Fact]
    public async Task Delete_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var createResponse = await AuthorizedSendAsync(new StuffPartRequest { ParentId = parent.Id, PartId = part.Id }, HttpMethod.Post);
        Assert.True(createResponse.IsSuccessStatusCode);

        // Act
        var result = await AuthorizedSendAsync(null, HttpMethod.Delete, $"{ApiRoutes.StuffParts}/{parent.Id}/{part.Id}");

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var createResponse = await AuthorizedSendAsync(new StuffPartRequest { ParentId = parent.Id, PartId = part.Id }, HttpMethod.Post);
        Assert.True(createResponse.IsSuccessStatusCode);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.StuffParts}/{parent.Id}/{part.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}