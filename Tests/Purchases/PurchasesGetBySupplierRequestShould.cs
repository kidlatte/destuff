using System.Collections.Generic;

namespace Destuff.Tests.Purchases;

public class PurchasesGetBySupplierRequestShould : IntegrationTestBase
{
    public PurchasesGetBySupplierRequestShould() : base(HttpMethod.Get, ApiRoutes.Purchases)
    {
    }

    [Fact]
    public async Task Get_Purchases()
    {
        // Arrange
        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier01a", Name = "Supplier 01a" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest() { SupplierId = supplier.Id }, HttpMethod.Post);
        Assert.NotNull(purchase);

        // Act
        var result = await AuthorizedGetAsync<PagedList<PurchaseBasicModel>>($"{ApiRoutes.PurchasesBySupplier}/{supplier?.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.List);

        var single = result.List.First();
        Assert.Equal(purchase?.Id, single?.Id);
    }
}