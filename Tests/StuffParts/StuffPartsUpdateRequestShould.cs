namespace Destuff.Tests.StuffParts;

public class StuffPartsUpdateRequestShould : IntegrationTestBase
{
    public StuffPartsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.StuffParts)
    {
    }

    [Fact]
    public async Task Update_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var partA = await AuthorizedSendAsync<StuffModel>(new LocationRequest { Name = "Part A" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(partA);

        var partB = await AuthorizedSendAsync<StuffModel>(new LocationRequest { Name = "Part B" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(partB);

        var create = new StuffPartRequest { Count = 1, PartId = partA.Id, ParentId = parent.Id };
        var created = await AuthorizedSendAsync<StuffPartModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        // Act 01
        var update01 = new StuffPartRequest { Count = 2, PartId = partA.Id, ParentId = parent.Id };
        var updated01 = await AuthorizedSendAsync<StuffPartModel>(update01, HttpMethod.Put, $"{ApiRoutes.StuffParts}/{created.Parent.Id}/{created.Part.Id}");

        // Assert 01
        Assert.NotNull(updated01);
        Assert.Equal(update01.Count, updated01.Count);
        Assert.Equal(update01.PartId, updated01.Part.Id);

        // Act 02
        var update02 = new StuffPartRequest { Count = 2, PartId = partB.Id, ParentId = parent.Id };
        var updated02 = await AuthorizedSendAsync<StuffPartModel>(update02, HttpMethod.Put, $"{ApiRoutes.StuffParts}/{created.Parent.Id}/{created.Part.Id}");

        // Assert 02
        Assert.NotNull(updated02);
        Assert.Equal(update02.Count, updated02.Count);
        Assert.Equal(update02.PartId, updated02.Part.Id);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_StuffPart()
    {
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var create = new StuffPartRequest { Count = 1, ParentId = parent.Id, PartId = part.Id };
        var created = await AuthorizedSendAsync<StuffPartModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        // Act
        var update = new StuffPartRequest { Count = 2, ParentId = parent.Id, PartId = part.Id };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.StuffParts}/{parent.Id}/{part.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}