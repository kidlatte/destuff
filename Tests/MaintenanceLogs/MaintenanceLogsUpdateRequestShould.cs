namespace Destuff.Tests.MaintenanceLogs;

public class MaintenanceLogsUpdateRequestShould : IntegrationTestBase
{
    public MaintenanceLogsUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.MaintenanceLogs)
    {
    }

    [Fact]
    public async Task Update_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id, DateTime = new DateTime(2000, 1, 1) };
        var model = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new MaintenanceLogRequest { MaintenanceId = maintenance.Id, DateTime = new DateTime(2000, 2, 2) };
        var result = await AuthorizedPutAsync<MaintenanceLogModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.DateTime, result.DateTime);
    }

    [Fact]
    public async Task Fail_Null_Maintenance_Stuff_Update_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };
        var model = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new MaintenanceLogRequest();
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_MaintenanceLog()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var create = new MaintenanceLogRequest { MaintenanceId = maintenance.Id, DateTime = new DateTime(2000, 1, 1) };
        var model = await AuthorizedSendAsync<MaintenanceLogModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new MaintenanceLogRequest { MaintenanceId = maintenance.Id, DateTime = new DateTime(2000, 2, 2) };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.MaintenanceLogs}/{model?.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}