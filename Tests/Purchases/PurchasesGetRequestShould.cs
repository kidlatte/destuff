namespace Destuff.Tests.Purchases;

public class PurchasesGetRequestShould : IntegrationTestBase
{
    public PurchasesGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Get_Purchases()
    {
        // Arrange
        var model = new PurchaseRequest();
        await AuthorizedSendAsync<PurchaseModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<PagedList<PurchaseListItem>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Supplier 01b", null, null, null, null, null)]
    [InlineData(3, null, null, null, null, "Received", null)]
    [InlineData(3, null, null, 2, 1, "Received", SortDirection.Descending)]
    [InlineData(2, "Supplier 01a", "supplier", null, null, "Received", null)]
    public async Task Get_Purchases_WithPaging(int count, string? supplierName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var supplierA = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier01a", Name = "Supplier 01a" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplierA);

        var supplierB = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier01b", Name = "Supplier 01b" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplierB);

        await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { Receipt = DateTime.Today.AddHours(1), Received = DateTime.Today.AddHours(4), SupplierId = supplierA.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { Receipt = DateTime.Today.AddHours(2), Received = DateTime.Today.AddHours(5), SupplierId = supplierB.Id }, HttpMethod.Post);
        await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest { Receipt = DateTime.Today.AddHours(3) }, HttpMethod.Post);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<PurchaseListItem>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(supplierName, result.List.First().Supplier?.Name);
    }
}