namespace Destuff.Tests.Users;

public class UsersDeleteRequestShould : IntegrationTestBase
{
    public UsersDeleteRequestShould() : base(HttpMethod.Delete, ApiRoutes.Users)
    {
    }

    [Fact]
    public async Task Delete_User()
    {
        var request = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        var register = await SendAsync<IdentityResultModel>(request, HttpMethod.Post, ApiRoutes.AuthRegister);
        Assert.True(register?.Succeeded);

        var result = await AuthorizedDeleteAsync(request.UserName);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Unauthorized_Delete_User()
    {
        // Arrange
        var request = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        var register = await SendAsync<IdentityResultModel>(request, HttpMethod.Post, ApiRoutes.AuthRegister);
        Assert.True(register?.Succeeded);

        // Act
        var result = await SendAsync(null, HttpMethod.Delete, $"{ApiRoutes.Users}/{request.UserName}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

}