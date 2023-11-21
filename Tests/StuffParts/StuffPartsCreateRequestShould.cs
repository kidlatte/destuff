using Destuff.Server.Data.Entities;

namespace Destuff.Tests.StuffParts;

public class StuffPartsCreateRequestShould : IntegrationTestBase
{
    public StuffPartsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.StuffParts)
    {
    }

    [Fact]
    public async Task Create_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var model = new StuffPartRequest { ParentId = parent.Id, PartId = part.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Return_StuffAndLocation()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var model = new StuffPartRequest { ParentId = parent.Id, PartId = part.Id };

        // Act
        var result = await AuthorizedSendAsync<StuffPartModel>(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(parent.Id, result.Parent.Id);
        Assert.Equal(part.Id, result.Part.Id);
    }

    [Fact]
    public async Task Fail_Null_Location_Create_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var model = new StuffPartRequest { ParentId = parent.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_StuffPart()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var model = new StuffPartRequest { ParentId = parent.Id, PartId = part.Id };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}