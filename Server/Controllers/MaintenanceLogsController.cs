using AutoMapper.QueryableExtensions;
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
    public MaintenanceLogsController(ControllerParameters<MaintenanceLog> param) : base(param)
    {
    }


    [HttpGet(ApiRoutes.MaintenanceLogsByMaintenance)]
    public async Task<PagedList<MaintenanceLogListItem>> GetByMaintenance(string hash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Maintenance> hasher)
    {
        var maintenanceId = hasher.Decode(hash);
        var query = Query.Where(x => x.MaintenanceId == maintenanceId);

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

    internal override async Task BeforeCreateAsync(MaintenanceLog entity, MaintenanceLogRequest _)
    {
        var maintenance = await Context.Maintenances.Where(x => x.Id == entity.MaintenanceId).FirstAsync();
        entity.Name = maintenance.Name;
        entity.StuffId = maintenance.StuffId;
    }
}
