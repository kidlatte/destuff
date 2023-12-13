namespace Destuff.Tests.Maintenances;

public class MaintenancesGetRequestShould : IntegrationTestBase
{
    public MaintenancesGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Maintenances)
    {
    }

    [Fact]
    public async Task Get_Maintenances()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var create = new MaintenanceRequest { StuffId = stuff.Id, Name = "Maintenance", EveryXDays = 1 };
        await AuthorizedSendAsync<MaintenanceModel>(create, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<PagedList<MaintenanceListItem>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Maintenance 003", null, null, null, null, null)]
    [InlineData(1, "Maintenance 002", "002", null, null, null, null)]
    [InlineData(3, "Maintenance 001", null, null, null, nameof(MaintenanceListItem.Name), null)]
    [InlineData(3, "Maintenance 002", null, 1, 1, null, null)]
    public async Task Get_Maintenances_WithPaging(int count, string? name, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff A" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff B" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { Name = "Maintenance 001", StuffId = stuffA.Id, EveryXDays = 1 }, HttpMethod.Post);
        await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { Name = "Maintenance 002", StuffId = stuffA.Id, EveryXDays = 1 }, HttpMethod.Post);
        await AuthorizedSendAsync<MaintenanceModel>(new MaintenanceRequest { Name = "Maintenance 003", StuffId = stuffB.Id, EveryXDays = 1 }, HttpMethod.Post);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<MaintenanceListItem>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(name, result.List.First().Name);
    }
}