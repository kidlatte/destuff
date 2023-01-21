namespace Destuff.Tests.Suppliers;

public class SuppliersDeleteRequestShould : IntegrationTestBase
{
    public SuppliersDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Suppliers)
    {
    }

    [Fact]
    public async Task Delete_Supplier()
    {
        var create = new SupplierCreateModel { ShortName = "supplier01", Name = "Supplier 001" };
        var created = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(created);

        var getResult = await AuthorizedGetAsync($"{ApiRoutes.Suppliers}/{created.Id}");
        Assert.True(getResult.IsSuccessStatusCode);

        var deleteResult = await AuthorizedDeleteAsync(created.Id!);
        Assert.True(deleteResult.IsSuccessStatusCode);

        getResult = await AuthorizedGetAsync($"{ApiRoutes.Suppliers}/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_Supplier()
    {
        // Arrange
        var create = new SupplierCreateModel { ShortName = "supplier02", Name = "Supplier 002" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Suppliers}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}