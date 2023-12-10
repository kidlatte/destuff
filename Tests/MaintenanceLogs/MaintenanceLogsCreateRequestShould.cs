using Destuff.Server.Data.Entities;

namespace Destuff.Tests.MaintenanceLogs;

public class MaintenanceLogsCreateRequestShould : IntegrationTestBase
{
    public MaintenanceLogsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.MaintenanceLogs)
    {
    }

    [Fact]
    public async Task Create_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task InheritMaintenanceName_OnCreateMaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance 001" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await AuthorizedSendAsync<MaintenanceLogModel>(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(maintenance.Name, result.Name);
    }

    [Fact]
    public async Task Fail_Null_Maintenance_Create_MaintenanceLog()
    {
        // Arrange
        var model = new MaintenanceLogRequest { };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}