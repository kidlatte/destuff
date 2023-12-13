namespace Destuff.Tests.Maintenances;

public class MaintenancesUpdateRequestShould : IntegrationTestBase
{
    public MaintenancesUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Update_Maintenance()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance A", EveryXDays = 1 };
        var model = await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);
        Assert.NotNull(model);
        Assert.Equal(create.Name, model.Name);

        // Act
        var update = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance B", EveryXDays = 1 };
        var result = await AuthorizedPutAsync<MaintenanceModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result.Name);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Maintenance()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance A", EveryXDays = 1 };
        var model = await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new MaintenanceRequest();
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Maintenances}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}