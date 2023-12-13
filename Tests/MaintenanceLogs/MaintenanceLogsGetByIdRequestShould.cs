namespace Destuff.Tests.MaintenanceLogs;

public class MaintenanceLogsGetByIdRequestShould : IntegrationTestBase
{
    public MaintenanceLogsGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.MaintenanceLogs)
    {
    }

    [Fact]
    public async Task Get_MaintenanceLog_Details()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };
        var model = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<MaintenanceLogModel>($"{ApiRoutes.MaintenanceLogs}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task InheritMaintenanceStuff_OnGetMaintenanceLogDetails()
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
        var result = await AuthorizedGetAsync<MaintenanceLogModel>($"{ApiRoutes.MaintenanceLogs}/{model.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stuff.Id, result.Stuff.Id);
    }

    [Fact]
    public async Task Fail_Nonexistent_MaintenanceLog_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.MaintenanceLogs}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}