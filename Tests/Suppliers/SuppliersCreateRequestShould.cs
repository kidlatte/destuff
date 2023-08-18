namespace Destuff.Tests.Suppliers;

public class SuppliersCreateRequestShould : IntegrationTestBase
{
    public SuppliersCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Suppliers)
    {
    }

    [Fact]
    public async Task Create_Supplier()
    {
        // Arrange
        var model = new SupplierRequest { ShortName = "supplier01", Name = "Supplier 001" };

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Supplier()
    {
        // Arrange
        var create = new SupplierRequest { ShortName = "Existing Slug", Name = "Supplier 001" };
        var created = await AuthorizedSendAsync(create);
        Assert.True(created?.IsSuccessStatusCode);

        // Act
        var sameSlug = new SupplierRequest { ShortName = "existing - slug", Name = "Supplier 002" };
        var result = await AuthorizedSendAsync(sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Create_Supplier()
    {
        // Arrange
        var model = new SupplierRequest();

        // Act
        var result = await AuthorizedSendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Create_Supplier()
    {
        // Arrange
        var model = new SupplierRequest { ShortName = "unauth", Name = "Unauthorized" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}