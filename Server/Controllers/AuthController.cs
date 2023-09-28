using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Auth)]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    [HttpPost, Route(ApiRoutes.AuthLogin)]
    public async Task<ActionResult<AuthModel>> Login([FromBody] LoginRequest model)
    {
        _logger.LogInformation("Login: {User}", model.UserName);

        if (!ModelState.IsValid || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null || user.UserName == null)
            return BadRequest();

        var isvalid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!isvalid)
            return BadRequest(new ErrorModel { Message = "Username or password is incorrect" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity
            (
                new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                }
            ),
            Expires = model.Remember ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new AuthModel
        {
            UserName = user.UserName,
            Token = tokenString,
            Expires = tokenDescriptor.Expires
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<IdentityResultModel>> Register([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            return BadRequest();

        try
        {
            var user = new ApplicationUser { UserName = model.UserName, Settings = new() };
            var result = await _userManager.CreateAsync(user, model.Password);
            return Ok(new IdentityResultModel
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(x => x.Description).ToList()
            });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new ErrorModel { Message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut, Route(ApiRoutes.AuthChangePassword)]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordRequest model)
    {

        if (!ModelState.IsValid || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            return BadRequest();

        try
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return BadRequest();

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, model.Password);
            return Ok(new IdentityResultModel
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(x => x.Description).ToList()
            });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { message = ex.Message });
        }
    }
}
