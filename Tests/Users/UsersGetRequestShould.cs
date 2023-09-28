namespace Destuff.Tests.Users;

public class UsersGetRequestShould : IntegrationTestBase
{
    public UsersGetRequestShould() : base(HttpMethod.Get, ApiRoutes.Users)
    {
    }

    [Fact]
    public async Task Get_Users()
    {
        // Arrange
        var request = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        var register = await SendAsync<IdentityResultModel>(request, HttpMethod.Post, ApiRoutes.AuthRegister);
        Assert.True(register?.Succeeded);

        // Act
        var result = await AuthorizedSendAsync<PagedList<UserModel>>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.List);
        Assert.Contains(request.UserName, result.List.Select(x => x.UserName));
    }

    [Theory]
    [InlineData(5, "admin", null, null, null, null, null)]
    [InlineData(2, "Search01", "search", null, null, null, null)]
    [InlineData(2, "Search02", "search", 1, 1, null, null)]
    [InlineData(2, "Search02", "search", null, null, "UserName", SortDirection.Descending)]
    public async Task Get_Users_WithPaging(int count, string firstName, string? search, int? page, int? pageSize, string? sortField, SortDirection? sortDir)
    {
        // Arrange
        await SendAsync(new RegisterRequest { UserName = "UserName", Password = "Qwer1234!" }, HttpMethod.Post, ApiRoutes.AuthRegister);
        await SendAsync(new RegisterRequest { UserName = "Search01", Password = "Qwer1234!" }, HttpMethod.Post, ApiRoutes.AuthRegister);
        await SendAsync(new RegisterRequest { UserName = "Search02", Password = "Qwer1234!" }, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var query = new ListQuery 
        { 
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortField = sortField,
            SortDir = sortDir ?? default
        };
        var result = await AuthorizedGetAsync<PagedList<UserModel>>($"{Route}?{query}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(count, result.Count);
        Assert.NotEmpty(result.List);
        Assert.Equal(firstName, result.List.First().UserName);
    }
}