namespace Destuff.Tests.Maintenances;

public class MaintenancesCreateRequestShould : IntegrationTestBase
{
    public MaintenancesCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Create_Maintenance()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var model = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance" };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Maintenance()
    {
        // Arrange
        var model = new MaintenanceRequest();

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}