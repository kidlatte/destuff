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

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
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

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance 001", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await AuthorizedSendAsync<MaintenanceLogModel>(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(maintenance.Name, result.Name);
    }

    [Fact]
    public async Task UpdateMaintenanceNext_OnCreateMaintenanceLog()
    {
        // Arrange
        MockDateTime.SetUtcNow(new DateTime(2000, 1, 1));

        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance 001", EveryXDays = 7 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await AuthorizedSendAsync(model);
        Assert.True(result.IsSuccessStatusCode);

        maintenance = await AuthorizedGetAsync<MaintenanceModel>($"{ApiRoutes.Maintenances}/{maintenance.Id}");

        // Assert
        Assert.NotNull(maintenance);
        Assert.Equal(new DateTime(2000, 1, 8), maintenance.Next);
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

        var maintenance = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 }, HttpMethod.Post, ApiRoutes.Maintenances);
        Assert.NotNull(maintenance);

        var model = new MaintenanceLogRequest { MaintenanceId = maintenance.Id };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}