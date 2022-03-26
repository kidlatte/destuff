using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Destuff.Server.Data;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Auth;

public class AuthLoginRequestShould: BaseIntegrationTest
{

    public AuthLoginRequestShould() : base(HttpMethod.Post, ApiRoutes.AuthLogin)
    {
    }

    [Fact]
    public async Task Login_Registered_User()
    {
        // Arrange
        var register = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new LoginModel { UserName = "user01", Password = "Qwer1234!" };
        var result = await SendAsync<AuthTokenModel>(model);

        // Assert
        Assert.Equal(model.UserName, result?.UserName);
    }

    [Fact]
    public async Task Fail_Unregistered_User()
    {
        // Arrange
        var model = new LoginModel { UserName = "user01", Password = "Qwer1234!" };

        // Act
        var result = await SendAsync(model);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Fail_Incorrect_Password()
    {
        // Arrange
        var register = new RegisterModel { UserName = "user01", Password = "Qwer1234!" };
        await SendAsync(register, HttpMethod.Post, ApiRoutes.AuthRegister);

        // Act
        var model = new LoginModel { UserName = "user01", Password = "Xwer1234!" };
        var result = await SendAsync(model);

        // Assert
        Assert.False(result.IsSuccessStatusCode);
    }
}