namespace Destuff.Tests.Maintenances;

public class MaintenancesGetByStuffRequestShould : IntegrationTestBase
{
    public MaintenancesGetByStuffRequestShould() : base(HttpMethod.Get, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Get_Maintenances()
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff A" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff B" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var maintenance01 = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuffA.Id, Name = "Maintenance 01", EveryXDays = 1 }, HttpMethod.Post);
        Assert.NotNull(maintenance01);

        var maintenance02 = await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { StuffId = stuffB.Id, Name = "Maintenance 02", EveryXDays = 1 }, HttpMethod.Post);
        Assert.NotNull(maintenance02);

        // Act
        var results = await AuthorizedGetAsync<PagedList<MaintenanceListItem>>(ApiRoutes.QueryMaintenancesByStuff(stuffB.Id, new()));

        // Assert
        Assert.NotNull(results);
        Assert.Single(results.List);

        var result = results.List.First();
        Assert.Equal(maintenance02.Id, result.Id);
    }
}