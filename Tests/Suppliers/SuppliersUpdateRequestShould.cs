namespace Destuff.Tests.Suppliers;

public class SuppliersUpdateRequestShould : IntegrationTestBase
{
    public SuppliersUpdateRequestShould() : base(HttpMethod.Put, ApiRoutes.Suppliers)
    {
    }

    [Fact]
    public async Task Update_Supplier()
    {
        // Arrange
        var create = new SupplierRequest { ShortName = "supplier01a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierRequest { ShortName = "supplier01b", Name = "Updated Supplier" };
        var result = await AuthorizedPutAsync<SupplierModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result?.Name);
    }

    [Fact]
    public async Task Fail_Existing_Slug_Create_Supplier()
    {
        // Arrange
        var create = new SupplierRequest { ShortName = "Existing Slug", Name = "Supplier 001" };
        var created = await AuthorizedSendAsync(create, HttpMethod.Post);
        Assert.True(created?.IsSuccessStatusCode);

        var newSlug = new SupplierRequest { ShortName = "New Slug", Name = "Supplier 002" };
        var model = await AuthorizedSendAsync<SupplierModel>(newSlug, HttpMethod.Post);
        Assert.NotNull(model?.Id);

        // Act
        var sameSlug = new SupplierRequest { ShortName = "existing - slug", Name = "Supplier 003" };
        var result = await AuthorizedPutAsync(model?.Id!, sameSlug);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Null_Name_Update_Supplier()
    {
       // Arrange
        var create = new SupplierRequest { ShortName = "supplier02a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierRequest { ShortName = "supplier02b", Name = null };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Supplier()
    {
        // Arrange
        var create = new SupplierRequest { ShortName = "supplier03a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierRequest { ShortName = "supplier03b", Name = "Updated Supplier" };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Suppliers}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}