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
using System.Collections.Generic;
using System.Linq;
using static MudBlazor.CategoryTypes;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Events)]
[ApiController, Authorize]
public class EventsController : BaseController<Event>
{
    public EventsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Event> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpGet]
    public async Task<PagedList<EventListItem>> Get([FromQuery] ListRequest? request)
    {
        var query = Query;

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
            .ProjectTo<EventListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<EventListItem>(count, list);
    }


    [HttpGet(ApiRoutes.EventsByStuff + "/{stuffHash}")]
    public async Task<PagedList<EventListItem>> GetByStuff(string stuffHash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Stuff> hasher)
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
            .ProjectTo<EventListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<EventListItem>(count, list);
    }
}
