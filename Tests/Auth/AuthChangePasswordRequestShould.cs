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
        var register = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        var model = new PasswordRequest { UserName = "user01", Password = "Xwer1234!" };
        await AuthorizedSendAsync<IdentityResultModel>(model);

        var login = new LoginRequest { UserName = "user01", Password = "Xwer1234!" };
        var auth = await SendAsync<AuthModel>(login, HttpMethod.Post, ApiRoutes.AuthLogin);

        // Assert
        Assert.NotNull(auth?.Token);
    }


    [Fact]
    public async Task Fail_Unauthorized_Password_Change()
    {
        // Arrange
        var register = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new PasswordRequest { UserName = "user01", Password = "Xwer1234!" };
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
        var register = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);


        // Act
        var model = new PasswordRequest { UserName = "user01", Password = password };
        var result = await AuthorizedSendAsync<IdentityResultModel>(model);

        // Assert
        Assert.False(result?.Succeeded);
    }

}