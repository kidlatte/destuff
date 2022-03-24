using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Shared;
using Destuff.Shared.Models;
using Destuff.Server.Data;
using BlazorGrid.Abstractions;
using BlazorGrid.Abstractions.Extensions;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Users)]
[ApiController]
public class UsersController : BaseController
{
    public UsersController(ApplicationDbContext context): base(context)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Index(BlazorGridRequest request)
    {
        var query = Context.Users.AsQueryable();

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
        if (request.OrderBy != null)
        {
            if (request.OrderByDescending)
                query = query.OrderByDescending(request.OrderBy);
            else
                query = query.OrderBy(request.OrderBy);
        }

        // Apply paging
        var rows = query.Skip(request.Offset).Take(request.Length)
            .Select(x => new UserModel { UserName = x.UserName }).ToList();

        return Ok(new BlazorGridResult<UserModel>
        {
            Data = rows,
            TotalCount = count
        });
    }
}