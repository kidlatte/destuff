namespace Destuff.Tests.StuffLocations;

public class StuffLocationsGetRequestShould : IntegrationTestBase
{
    public StuffLocationsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.StuffLocations)
    {
    }

    [Fact]
    public async Task Get_StuffLocations()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        var model = new StuffLocationRequest { LocationId = location.Id, StuffId = stuff.Id };
        await AuthorizedSendAsync<StuffLocationModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<PagedList<StuffLocationModel>>($"{ApiRoutes.StuffLocations}/{location.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Stuff 001", null, null, null, null, null)]
    [InlineData(3, "Stuff 003", null, null, null, "Stuff", SortDirection.Descending)]
    [InlineData(3, "Stuff 002", null, 1, 1, "Count", null)]
    [InlineData(1, "Stuff 002", "002", null, null, null, null)]
    public async Task Get_StuffLocations_WithPaging(int count, string stuffName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 002" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var stuffC = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 003" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffC);

        var location = await AuthorizedSendAsync<LocationModel>(new LocationRequest { Name = "New Location" }, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(location);

        await AuthorizedSendAsync<StuffLocationModel>(new StuffLocationRequest { Count = 1, StuffId = stuffA.Id, LocationId = location.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<StuffLocationModel>(new StuffLocationRequest { Count = 2, StuffId = stuffB.Id, LocationId = location.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<StuffLocationModel>(new StuffLocationRequest { Count = 3, StuffId = stuffC.Id, LocationId = location.Id }, HttpMethod.Post);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<StuffLocationModel>>($"{ApiRoutes.StuffLocations}/{location.Id}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(stuffName, result.List.First().Stuff?.Name);
    }
}