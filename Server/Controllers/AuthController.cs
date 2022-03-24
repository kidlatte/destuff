using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BlazorGrid.Abstractions;
using BlazorGrid.Abstractions.Extensions;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace Destuff.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppSettings _appSettings;
    ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    [HttpPost, Route(ApiRoutes.AuthLogin)]
    public async Task<ActionResult<AuthTokenModel>> Login([FromBody] LoginModel model)
    {
        _logger.LogInformation("Login: {User}", model.UserName);

        var user = await _userManager.FindByNameAsync(model.UserName);
        var isvalid = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!isvalid)
            return BadRequest(new { message = "Username or password is incorrect" });

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

        return Ok(new AuthTokenModel
        {
            UserName = user.UserName,
            AuthToken = tokenString,
            Expires = tokenDescriptor.Expires
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResultModel>> Register([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        try
        {
            var user = new ApplicationUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);
            return Ok(new RegisterResultModel
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

    [Authorize]
    [HttpPost, Route(ApiRoutes.AuthUsers)]
    public async Task<IActionResult> FetchUsers(BlazorGridRequest request)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Query))
            query = query.Where(x => x.UserName.StartsWith(request.Query));

        var count = await query.CountAsync();
        if (count == 0)
        {
            return Ok(new BlazorGridResult<UserModel> 
            {
                Data = new List<UserModel>(),
                TotalCount = 0
            });
        }

        // Apply ordering
        if (request.OrderBy == null)
            query = query.OrderBy(x => x.UserName);
        else if (request.OrderByDescending)
            query = query.OrderByDescending(request.OrderBy);
        else
            query = query.OrderBy(request.OrderBy);

        // Apply paging
        var rows = query.Skip(request.Offset).Take(request.Length)
            .Select(x => new UserModel { UserName = x.UserName }).ToList();

        return Ok(new BlazorGridResult<UserModel>
        {
            Data = rows,
            TotalCount = count
        });
    }

    [Authorize]
    [HttpPost, Route(ApiRoutes.AuthChangePassword)]
    public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, model.Password);
        return Ok();
    }

    [HttpGet]
    [Route(ApiRoutes.AuthOnline)]
    public async Task<IActionResult> CheckOnline()
    {
        await Task.CompletedTask;
        return Ok(new { Online = true });
    }
}
