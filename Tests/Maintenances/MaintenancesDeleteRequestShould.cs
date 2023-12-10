namespace Destuff.Tests.Maintenances;

public class MaintenancesDeleteRequestShould : IntegrationTestBase
{
    public MaintenancesDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Delete_Maintenance()
    {
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" };
        var created = await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.Maintenances}/{created.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.Maintenances}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_Maintenance()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" };
        var model = await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Maintenances}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}