using AutoMapper;
using Destuff.Server.Data;
using Destuff.Server.Models;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Users)]
[ApiController]
[Authorize]
public class UsersController : BaseController
{
    public UsersController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    [HttpGet]
    public async Task<PagedList<UserModel>> Get([FromQuery] ListRequest? request)
    {
        var query = Context.Users.AsQueryable();

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.UserName!.ToLower().Contains(search)));
        }

        switch (request.SortField)
        {
            case "UserName":
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName);
                break;
            default:
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .Select(x => new UserModel { UserName = x.UserName ?? "" })
            .ToListAsync();

        return new (count, list);
    }

    [HttpGet(ApiRoutes.UserSettings)]
    public async Task<ActionResult<UserSettings>> GetSettings()
    {
        var user = await Context.Users.FirstOrDefaultAsync(x => x.UserName == CurrentUserName);
        if (user == null)
            return BadRequest();

        if (user.Settings != null)
            return user.Settings;

        user.Settings = new() { InventoryEnabled = false, PurchasesEnabled = false };
        await Context.SaveChangesAsync();
        return user.Settings;
    }

    [HttpPut(ApiRoutes.UserSettings)]
    public async Task<ActionResult<UserSettings>> UpdateSettings([FromBody] UserSettings settings)
    {
        var user = await Context.Users.FirstOrDefaultAsync(x => x.UserName == CurrentUserName);
        if (user == null)
            return BadRequest();

        user.Settings = settings;
        await Context.SaveChangesAsync();

        return settings;
    }


    [HttpDelete("{userName}")]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        var user = await Context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
            return BadRequest();

        var result = Context.Users.Remove(user);
        await Context.SaveChangesAsync();

        return Ok(new IdentityResultModel
        {
            Succeeded = true
        });
    }
}
