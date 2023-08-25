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
        var create = new StuffRequest { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffRequest { Name = "Updated Stuff" };
        var result = await AuthorizedPutAsync<StuffModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result?.Name);
    }

    [Fact]
    public async Task UpdateLocation_WhenProvided()
    {
        // Arrange
        var locationA = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "Location A" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationA);

        var locationB = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "Location B" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(locationB);

        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Created Stuff" }, HttpMethod.Post);
        Assert.NotNull(stuff);
        Assert.Null(stuff.FirstLocation);

        // Act
        var resultA = await AuthorizedPutAsync<StuffModel>(stuff.Id, new StuffRequest() { Name = "Updated Stuff", LocationId = locationA.Id });
        Assert.NotNull(resultA);
        Assert.Equal(locationA.Id, resultA.FirstLocation?.Id);

        var resultB = await AuthorizedPutAsync<StuffModel>(stuff.Id, new StuffRequest() { Name = "Updated Stuff", LocationId = locationB.Id });
        Assert.NotNull(resultB);
        Assert.Equal(locationB.Id, resultB.FirstLocation?.Id);

        await AuthorizedPutAsync(stuff.Id, new StuffRequest() { Name = "Updated Stuff", LocationId = locationB.Id });
        var resultC = await AuthorizedGetAsync<StuffModel>($"{ApiRoutes.Stuffs}/{stuff.Id}");
        Assert.NotNull(resultC);
        Assert.Equal(locationB.Id, resultC.FirstLocation?.Id);

        var resultD = await AuthorizedPutAsync<StuffModel>(stuff.Id, new StuffRequest() { Name = "Updated Stuff" });
        Assert.NotNull(resultD);
        Assert.Null(resultD.FirstLocation);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Stuff()
    {
        // Arrange
        var create = new StuffRequest { Name = "Existing Slug" };
        var created = await AuthorizedSendAsync(create, HttpMethod.Post);
        Assert.True(created?.IsSuccessStatusCode);

        var newSlug = new StuffRequest { Name = "New Slug" };
        var model = await AuthorizedSendAsync<StuffModel>(newSlug, HttpMethod.Post);
        Assert.NotNull(model?.Id);

        // Act
        var sameSlug = new StuffRequest { Name = "existing - slug" };
        var result = await AuthorizedPutAsync(model?.Id!, sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Update_Stuff()
    {
       // Arrange
        var create = new StuffRequest { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffRequest { Name = null };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Stuff()
    {
        // Arrange
        var create = new StuffRequest { Name = "Created Stuff" };
        var model = await AuthorizedSendAsync<StuffModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new StuffRequest { Name = "Updated Stuff" };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Stuffs}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}