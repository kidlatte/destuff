using AutoMapper;
using BlazorGrid.Abstractions;
using BlazorGrid.Abstractions.Extensions;
using Destuff.Server.Data;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BlazorGridResult<UserModel>>> FetchUsers(BlazorGridRequest request)
    {
        var query = Context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Query))
            query = query.Where(x => x.UserName != null && x.UserName.StartsWith(request.Query));

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
