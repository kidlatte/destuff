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
public class EventsController : BaseController<Event, EventModel, EventRequest>
{
    public EventsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Event> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpGet]
    public async Task<PagedList<EventListItem>> Get([FromQuery] ListRequest? request)
    {
        request ??= new ListRequest();

        var eventsQuery = Query;
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                eventsQuery = eventsQuery.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var purchaseItemsQuery = Context.PurchaseItems.AsQueryable();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                purchaseItemsQuery = purchaseItemsQuery.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var query = eventsQuery.ProjectTo<EventBuffer>(Mapper.ConfigurationProvider)
            .Union(purchaseItemsQuery.ProjectTo<EventBuffer>(Mapper.ConfigurationProvider));

        var sortField = request.SortField ?? "";
        var desc = request.SortDir == SortDirection.Descending;
        query = sortField switch {
            "" => query.OrderByDescending(x => x.DateTime),
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
        request ??= new ListRequest();
        var stuffId = hasher.Decode(stuffHash);

        var eventsQuery = Query.Where(x => x.StuffId == stuffId);
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                eventsQuery = eventsQuery.Where(x => x.Summary!.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var purchaseItemsQuery = Context.PurchaseItems.Where(x => x.StuffId == stuffId);
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                purchaseItemsQuery = purchaseItemsQuery.Where(x => x.Summary!.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var query = eventsQuery.ProjectTo<EventBuffer>(Mapper.ConfigurationProvider)
            .Union(purchaseItemsQuery.ProjectTo<EventBuffer>(Mapper.ConfigurationProvider));

        var sortField = request.SortField ?? "";
        var desc = request.SortDir == SortDirection.Descending;
        query = sortField switch {
            "" => query.OrderByDescending(x => x.DateTime),
            _ => desc ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<EventListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<EventListItem>(count, list);
    }

    public override Task<ActionResult<EventModel>> Update(string hash, [FromBody] EventRequest request)
    {
        throw new InvalidOperationException();
    }

    internal override async Task BeforeSaveAsync(Event entity, EventRequest request)
    {
        entity.Data = entity.Type switch {
            EventType.Inventory => await GenerateInventoryData(entity),
            _ => await GenerateData(entity, request)
        };
    }

    async Task<EventData> GenerateData(Event entity, EventRequest request)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == entity.StuffId).FirstAsync();
        stuff.Inventoried = entity.DateTime;

        if (!string.IsNullOrEmpty(request.Recipient))
            entity.Summary = $"Has been {entity.Type} to {request.Recipient}";
        else
            entity.Summary = $"Has been {entity.Type}";

        return new EventData {
            Recipient = request.Recipient,
            Stuff = Mapper.Map<StuffBasicModel>(stuff),
        };
    }

    async Task<EventData> GenerateInventoryData(Event entity)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == entity.StuffId).FirstAsync();
        stuff.Inventoried = entity.DateTime;

        var locations = await Context.StuffLocations.Where(x => x.StuffId == entity.StuffId)
            .ProjectTo<StuffLocationListItem>(Mapper.ConfigurationProvider).ToListAsync();

        entity.Summary = GenerateInventorySummary(locations);

        return new EventData {
            Stuff = Mapper.Map<StuffBasicModel>(stuff),
            Locations = locations
        };
    }

    string GenerateInventorySummary(ICollection<StuffLocationListItem>? locations)
    {
        if (locations == null)
            return $"No location on record";

        var count = locations.Sum(x => x.Count);
        if (count == 0)
            return $"No location on record";

        var location = locations.First().Location;
        if (count == 1)
            return $"Located in {location.Name}";

        if (locations.Count == 1)
            return $"{count} units are in {location.Name}";

        return $"{count} total units are in {locations.Count} locations.";
    }
}
