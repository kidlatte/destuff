using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;
using System.Net;

namespace Destuff.Tests.Auth;

public class AuthChangePasswordRequestShould: IntegrationTestBase
{

    public AuthChangePasswordRequestShould() : base(HttpMethod.Put, ApiRoutes.AuthChangePassword)
    {
    }

    [Fact]
    public async Task Change_Password_Success()
    {
        var register = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        var model = new PasswordChangeModel { UserName = "user01", Password = "Xwer1234!" };
        await AuthorizedSendAsync<IdentityResultModel>(model);

        var login = new LoginModel { UserName = "user01", Password = "Xwer1234!" };
        var auth = await SendAsync<AuthTokenModel>(login, HttpMethod.Post, ApiRoutes.AuthLogin);

        // Assert
        Assert.NotNull(auth?.AuthToken);
    }


    [Fact]
    public async Task Fail_Unauthorized_Password_Change()
    {
        // Arrange
        var register = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new PasswordChangeModel { UserName = "user01", Password = "Xwer1234!" };
        var result = await SendAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result?.StatusCode);
    }

    [Theory]
    [InlineData("Qw12!")]
    [InlineData("Qwer12345")]
    [InlineData("qwer1234!")]
    [InlineData("QwerQwer!")]
    public async Task Fail_Weak_Password(string password)
    {
        // Arrange
        var register = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);


        // Act
        var model = new PasswordChangeModel { UserName = "user01", Password = password };
        var result = await AuthorizedSendAsync<IdentityResultModel>(model);

        // Assert
        Assert.False(result?.Succeeded);
    }

}