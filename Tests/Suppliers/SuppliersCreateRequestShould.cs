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