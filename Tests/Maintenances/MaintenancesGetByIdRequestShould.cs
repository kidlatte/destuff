namespace Destuff.Tests.Maintenances;

public class MaintenancesGetByIdRequestShould : IntegrationTestBase
{
    public MaintenancesGetByIdRequestShould() : base(HttpMethod.Get, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Get_Maintenance_Details()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" };
        var model = await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<MaintenanceModel>($"{ApiRoutes.Maintenances}/{model?.Id}");

        // Assert
        Assert.NotNull(result?.Id);
        Assert.Equal(model?.Id, result?.Id);
    }

    [Fact]
    public async Task Fail_Nonexistent_Maintenance_Details()
    {
        // Act
        var result = await AuthorizedGetAsync($"{ApiRoutes.Maintenances}/xxx");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}