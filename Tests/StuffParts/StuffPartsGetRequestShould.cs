namespace Destuff.Tests.StuffParts;

public class StuffPartsGetRequestShould : IntegrationTestBase
{
    public StuffPartsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.StuffParts)
    {
    }

    [Fact]
    public async Task Get_StuffParts()
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var part = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Part Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(part);

        var model = new StuffPartRequest { ParentId = parent.Id, PartId = part.Id };
        await AuthorizedSendAsync<StuffPartModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedGetAsync<PagedList<StuffPartListItem>>($"{ApiRoutes.StuffParts}/{parent.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Stuff 001", null, null, null, null, null)]
    [InlineData(3, "Stuff 003", null, null, null, "Part", SortDirection.Descending)]
    [InlineData(3, "Stuff 002", null, 1, 1, "Count", null)]
    [InlineData(1, "Stuff 002", "002", null, null, null, null)]
    public async Task Get_StuffParts_WithPaging(int count, string stuffName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var parent = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Parent Stuff" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(parent);

        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 002" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var stuffC = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 003" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffC);

        await AuthorizedSendAsync<StuffPartModel>(new StuffPartRequest { Count = 1, PartId = stuffA.Id, ParentId = parent.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<StuffPartModel>(new StuffPartRequest { Count = 2, PartId = stuffB.Id, ParentId = parent.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<StuffPartModel>(new StuffPartRequest { Count = 3, PartId = stuffC.Id, ParentId = parent.Id }, HttpMethod.Post);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<StuffPartListItem>>($"{ApiRoutes.StuffParts}/{parent.Id}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(stuffName, result.List.First().Part?.Name);
    }
}