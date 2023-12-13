namespace Destuff.Tests.MaintenanceLogs;

public class MaintenanceLogsDeleteRequestShould : IntegrationTestBase
{
    public MaintenanceLogsDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.MaintenanceLogs)
    {
    }

    [Fact]
    public async Task Delete_MaintenanceLog()
    {
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };
        var created = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.MaintenanceLogs}/{created.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.MaintenanceLogs}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };
        var model = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.MaintenanceLogs}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}