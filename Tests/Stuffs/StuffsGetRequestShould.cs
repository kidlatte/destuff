using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsGetRequestShould : IntegrationTestBase
{
    public StuffsGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Stuffs)
    {
    }

    [Fact]
    public async Task Get_Stuffs()
    {
        // Arrange
        var model = new StuffCreateModel { Name = "Stuff Name" };
        await AuthorizedSendAsync<StuffModel>(model, HttpMethod.Post);

        // Act
        var result = await AuthorizedSendAsync<PagedList<StuffModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count);
        Assert.Single(result.List);
    }

    [Theory]
    [InlineData(3, "Stuff Name", null, null, null, null, null)]
    [InlineData(2, "Search 01", "search", null, null, null, null)]
    [InlineData(2, "Search 02", "search", 1, 1, null, null)]
    [InlineData(2, "Search 02", "search", null, null, "name", SortDirection.Descending)]
    public async Task Get_Stuffs_WithPaging(int count, string firstName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        await AuthorizedSendAsync<StuffModel>(new StuffCreateModel { Name = "Stuff Name" }, HttpMethod.Post);
        var model = new StuffCreateModel { Name = "Search 01" };
        await AuthorizedSendAsync<StuffModel>(model, HttpMethod.Post);
        await AuthorizedSendAsync<StuffModel>(new StuffCreateModel { Name = "Search 02" }, HttpMethod.Post);

        // Act
        var query = new PagedQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<StuffModel>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(firstName, result.List.First().Name);
    }
}