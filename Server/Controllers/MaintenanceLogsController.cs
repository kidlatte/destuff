using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.MaintenanceLogs)]
[ApiController, Authorize]
public class MaintenanceLogsController : BaseController<MaintenanceLog, MaintenanceLogModel, MaintenanceLogRequest>
{
    public MaintenanceLogsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<MaintenanceLog> hasher) : base(context, mapper, hasher)
    {
    }


    [HttpGet]
    public async Task<PagedList<MaintenanceLogListItem>> Get([FromQuery] ListRequest? request)
    {
        var query = Query;

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.Name.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" =>
                query.OrderByDescending(x => x.Created),
            _ =>
                request.SortDir == SortDirection.Descending ?
                    query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<MaintenanceLogListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<MaintenanceLogListItem>(count, list);
    }
}
