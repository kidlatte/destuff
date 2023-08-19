using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;
using Destuff.Server.Data.Entities;

namespace Destuff.Tests.Stuffs;

public class StuffsGetBySupplierRequestShould : IntegrationTestBase
{
    public StuffsGetBySupplierRequestShould() : base(HttpMethod.Get, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Get_Stuffs()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff Name" }, HttpMethod.Post);
        Assert.NotNull(stuff);

        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier01a", Name = "Supplier 01a" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest() { SupplierId = supplier.Id }, HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItem = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuff.Id, Price = 1 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItem);

        // Act
        var result = await AuthorizedGetAsync<PagedList<StuffListItem>>($"{ApiRoutes.StuffsBySupplier}/{supplier.Id}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "003 Search", null, null, null, null, null)]
    [InlineData(3, "001 Stuff", null, null, null, "Name", null)]
    [InlineData(2, "002 Search", "search", null, null, "Name", null)]
    [InlineData(2, "002 Search", "search", 1, 1, "Name", SortDirection.Descending)]
    public async Task Get_Stuffs_WithPaging(int count, string firstName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        var stuffA = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "No Supplier" }, HttpMethod.Post);
        Assert.NotNull(stuffA);

        var stuffB = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "001 Stuff" }, HttpMethod.Post);
        Assert.NotNull(stuffB);

        var stuffC = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "002 Search" }, HttpMethod.Post);
        Assert.NotNull(stuffC);

        var stuffD = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "003 Search" }, HttpMethod.Post);
        Assert.NotNull(stuffD);

        var supplier = await AuthorizedSendAsync<SupplierModel>(new SupplierRequest { ShortName = "supplier01a", Name = "Supplier 01a" }, HttpMethod.Post, ApiRoutes.Suppliers);
        Assert.NotNull(supplier);

        var purchase = await AuthorizedSendAsync<PurchaseModel>(new PurchaseRequest() { SupplierId = supplier.Id }, HttpMethod.Post, ApiRoutes.Purchases);
        Assert.NotNull(purchase);

        var purchaseItemA = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuffB.Id, Price = 1 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItemA);

        var purchaseItemB = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuffC.Id, Price = 2 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItemB);

        var purchaseItemC = await AuthorizedSendAsync<PurchaseItemModel>(new PurchaseItemRequest { PurchaseId = purchase.Id, StuffId = stuffD.Id, Price = 3 }, HttpMethod.Post, ApiRoutes.PurchaseItems);
        Assert.NotNull(purchaseItemC);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<StuffListItem>>($"{ApiRoutes.StuffsBySupplier}/{supplier.Id}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(firstName, result.List.First().Name);
    }
}