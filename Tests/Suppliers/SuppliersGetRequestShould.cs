namespace Destuff.Tests.Suppliers;

public class SuppliersGetRequestShould : IntegrationTestBase
{
    public SuppliersGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Suppliers)
    {
    }

    [Fact]
    public async Task Get_Suppliers()
    {
        // Arrange
        var model = new SupplierCreateModel { ShortName = "supplier01", Name = "Supplier 001" };
        await AuthorizedSendAsync<SupplierModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<PagedList<SupplierModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Search 02", null, null, null, null, null)]
    [InlineData(2, "Search 02", "search", null, null, null, null)]
    [InlineData(2, "Search 01", "search", 1, 1, null, null)]
    [InlineData(2, "Search 02", "search", null, null, "name", SortDirection.Descending)]
    public async Task Get_Suppliers_WithPaging(int count, string firstName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        await AuthorizedSendAsync<SupplierModel>(new SupplierCreateModel { ShortName = "supplier", Name = "Supplier Name" }, HttpMethod.Post);
        await AuthorizedSendAsync<SupplierModel>(new SupplierCreateModel { ShortName = "search01", Name = "Search 01" }, HttpMethod.Post);
        await AuthorizedSendAsync<SupplierModel>(new SupplierCreateModel { ShortName = "search02", Name = "Search 02" }, HttpMethod.Post);

        // Act
        var query = new PagedQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<SupplierModel>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(firstName, result.List.First().Name);
    }
}