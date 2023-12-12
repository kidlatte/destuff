namespace Destuff.Tests.MaintenanceLogs;

public class MaintenanceLogsGetByMaintenanceRequestShould : IntegrationTestBase
{
    public MaintenanceLogsGetByMaintenanceRequestShould() : base(HttpMethod.Get, ApiRoutes.MaintenanceLogsByMaintenance)
    {
    }

    [Fact]
    public async Task Get_MaintenanceLogs()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var maintenanceLog = await AuthorizedSendAsync<MaintenanceLogModel>(new MaintenanceLogRequest { MaintenanceId = maintenance.Id }, HttpMethod.Post, ApiRoutes.MaintenanceLogs);
        Assert.NotNull(maintenanceLog);

        // Act
        var results = await AuthorizedGetAsync<PagedList<MaintenanceLogListItem>>(ApiRoutes.QueryMaintenanceLogsByMaintenance(maintenance.Id));

        // Assert
        Assert.NotNull(results);
        Assert.Single(results.List);

        var result = results.List.First();
        Assert.Equal(maintenanceLog.Id, result.Id);
    }
}