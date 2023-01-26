namespace Destuff.Tests.PurchaseItems;

public class PurchaseItemsGetRequestShould : IntegrationTestBase
{
    public PurchaseItemsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.PurchaseItems)
    {
    }

    [Fact]
    public async Task Get_PurchaseItems()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffCreateModel { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseCreateModel(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var model = new PurchaseItemCreateModel { PurchaseId = purchase.Id, StuffId = stuff.Id };
        await AuthorizedSendAsync<PurchaseItemModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<PagedList<PurchaseItemModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Stuff 001b", null, null, null, null, null)]
    [InlineData(3, "Stuff 001b", null, null, null, "quantity", SortDirection.Descending)]
    //[InlineData(3, "Stuff 001b", null, 1, 1, "cost", SortDirection.Ascending)]
    [InlineData(2, "Stuff 001a", "001a", null, null, null, null)]
    public async Task Get_PurchaseItems_WithPaging(int count, string firstName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffCreateModel { Name = "Stuff 001a" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffCreateModel { Name = "Stuff 001b" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuffB);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseCreateModel(), HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemCreateModel { Quantity = 1, Cost = 1, PurchaseId = purchase.Id, StuffId = stuffA.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemCreateModel { Quantity = 2, Cost = 5, PurchaseId = purchase.Id, StuffId = stuffA.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemCreateModel { Quantity = 3, Cost = 3, PurchaseId = purchase.Id, StuffId = stuffB.Id }, HttpMethod.Post);

        // Act
        var query = new PagedQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<PurchaseItemModel>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(firstName, result.List.First().Stuff?.Name);
    }
}