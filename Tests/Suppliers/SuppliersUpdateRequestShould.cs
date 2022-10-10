using Xunit;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

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
        var create = new SupplierCreateModel { ShortName = "supplier01a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierCreateModel { ShortName = "supplier01b", Name = "Updated Supplier" };
        var result = await AuthorizedPutAsync<SupplierModel>(model?.Id!, update);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(update.Name, result?.Name);
    }

    [Fact]
    public async Task Fail_Null_Name_Update_Supplier()
    {
       // Arrange
        var create = new SupplierCreateModel { ShortName = "supplier02a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierCreateModel { ShortName = "supplier02b", Name = null };
        var result = await AuthorizedPutAsync(model?.Id!, update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result?.StatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Update_Supplier()
    {
        // Arrange
        var create = new SupplierCreateModel { ShortName = "supplier03a", Name = "Created Supplier" };
        var model = await AuthorizedSendAsync<SupplierModel>(create, HttpMethod.Post);
        Assert.NotNull(model);

        // Act
        var update = new SupplierCreateModel { ShortName = "supplier03b", Name = "Updated Supplier" };
        var result = await SendAsync(update, HttpMethod.Put, $"{ApiRoutes.Suppliers}/{model?.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

}