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

[Route(ApiRoutes.Maintenances)]
[ApiController, Authorize]
public class MaintenancesController : BaseController<Maintenance, MaintenanceModel, MaintenanceRequest>
{
    public MaintenancesController(ControllerParameters<Maintenance> param) : base(param)
    {
    }

    [HttpGet]
    public async Task<PagedList<MaintenanceListItem>> Get([FromQuery] ListRequest? request)
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
            .ProjectTo<MaintenanceListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<MaintenanceListItem>(count, list);
    }

    [HttpGet(ApiRoutes.MaintenancesByStuff + "/{stuffHash}")]
    public async Task<PagedList<MaintenanceListItem>> GetByStuff(string stuffHash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Stuff> hasher)
    {
        var stuffId = hasher.Decode(stuffHash);
        var query = Query.Where(x => x.StuffId == stuffId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var sortField = request.SortField ?? "";
        var desc = request.SortDir == SortDirection.Descending;
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Created),
            _ => desc ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<MaintenanceListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<MaintenanceListItem>(count, list);
    }

    internal override Task BeforeCreateAsync(Maintenance entity, MaintenanceRequest request)
    {
        entity.Next = DateTimeProvider.UtcNow.AddDays(entity.EveryXDays);
        return base.BeforeCreateAsync(entity, request);
    }
}
