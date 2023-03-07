using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Auth;

public class AuthLoginRequestShould: IntegrationTestBase
{

    public AuthLoginRequestShould() : base(HttpMethod.Post, ApiRoutes.AuthLogin)
    {
    }

    [Fact]
    public async Task Login_Registered_User()
    {
        // Arrange
        var register = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new LoginRequest { UserName = "user01", Password = "Qwer1234!" };
        var result = await SendAsync<AuthModel>(model);

        // Assert
        Assert.Equal(model.UserName, result?.UserName);
    }

    [Fact]
    public async Task Fail_Unregistered_User()
    {
        // Arrange
        var model = new LoginRequest { UserName = "user01", Password = "Qwer1234!" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Incorrect_Password()
    {
        // Arrange
        var register = new RegisterRequest { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new LoginRequest { UserName = "user01", Password = "Xwer1234!" };
        var result = await SendAsync(model);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
    }
}