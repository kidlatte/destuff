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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.StuffLocations)]
[ApiController, Authorize]
public class StuffLocationsController : BaseController
{
    private IIdentityHasher<Location> LocationHasher { get; }
    private IIdentityHasher<Stuff> StuffHasher { get; }

    public StuffLocationsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Stuff> stuffHasher, IIdentityHasher<Location> locationHasher) : base(context, mapper)
    {
        StuffHasher = stuffHasher;
        LocationHasher = locationHasher;
    }

    [HttpGet(ApiRoutes.StuffLocations + "/{locationHash}")]
    public async Task<PagedList<StuffLocationModel>> Get(string locationHash, [FromQuery] ListRequest? request)
    {
        var locationId = LocationHasher.Decode(locationHash);

        var query = Context.StuffLocations.Where(x => x.LocationId == locationId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                x.Stuff!.Url!.ToLower().Contains(search) ||
                x.Stuff!.Notes!.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        switch (sortField) {
            case "":
                break;
            case nameof(StuffLocationModel.Stuff):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Stuff!.Name) : query.OrderBy(x => x.Stuff!.Name);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffLocationModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffLocationModel>(count, list);
    }

    [HttpPost]
    public async Task<ActionResult<StuffLocationModel>> Create([FromBody] StuffLocationRequest model)
    {
        if (!ModelState.IsValid || model.StuffId == null || model.LocationId == null)
            return BadRequest(model);

        var stuffId = StuffHasher.Decode(model.StuffId);
        var locationId = LocationHasher.Decode(model.LocationId);

        var exists = await Context.StuffLocations.AnyAsync(x => x.StuffId == stuffId && x.LocationId == locationId);
        if (exists)
            return BadRequest("Location already exists.");

        var entity = Mapper.Map<StuffLocation>(model);
        Context.Add(entity);
        await Context.SaveChangesAsync();

        return await Context.StuffLocations
            .Where(x => x.StuffId == stuffId && x.LocationId == locationId)
            .ProjectTo<StuffLocationModel>(Mapper.ConfigurationProvider)
            .FirstAsync();
    }

    [HttpPut("{stuffHash}/{locationHash}")]
    public async Task<ActionResult<StuffLocationModel>> Update(string stuffHash, string locationHash, [FromBody] StuffLocationRequest request)
    {
        if (!ModelState.IsValid || request.StuffId == null || request.LocationId == null)
            return BadRequest(request);
        
        StuffLocationModel? model = null;
        var stuffId = StuffHasher.Decode(stuffHash);
        var locationId = LocationHasher.Decode(locationHash);

        var entity = await Context.StuffLocations.FirstOrDefaultAsync(x => x.StuffId == stuffId && x.LocationId == locationId);
        if (entity == null)
            return BadRequest("Location does not exist.");

        if (locationHash == request.LocationId) {
            Mapper.Map(request, entity);
        }
        else 
        {
            // location moved
            var oldEntity = entity;
            Context.Remove(oldEntity);

            entity = Mapper.Map<StuffLocation>(request);
            Context.Add(entity);

            model = await CreateMovedEvent(oldEntity, entity);
        }

        await Context.SaveChangesAsync();

        model ??= await Context.StuffLocations
            .Where(x => x.StuffId == entity.StuffId && x.LocationId == entity.LocationId)
            .ProjectTo<StuffLocationModel>(Mapper.ConfigurationProvider)
            .FirstAsync();

        return model;
    }

    private async Task<StuffLocationModel> CreateMovedEvent(StuffLocation oldEntity, StuffLocation newEntity)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == newEntity.StuffId)
            .ProjectTo<StuffBasicModel>(Mapper.ConfigurationProvider).FirstAsync();

        var locations = await Context.Locations
            .Where(x => x.Id == oldEntity.LocationId || x.Id == newEntity.LocationId)
            .ToDictionaryAsync(x => x.Id, x => x);

        var oldLocation = Mapper.Map<LocationListItem>(locations[oldEntity.LocationId]);
        var newLocation = Mapper.Map<LocationListItem>(locations[newEntity.LocationId]);

        var eventEntity = new Event {
            Type = EventType.Move,
            StuffId = newEntity.StuffId,
            Count = newEntity.Count,
            DateTime = DateTime.UtcNow,
            Summary = $"Moved from {oldLocation.Name} to {newLocation.Name}.",
            Data = new EventData { 
                Stuff = stuff,
                FromLocation = oldLocation,
                ToLocation = newLocation
            }
        };
        Audit(eventEntity);
        Context.Add(eventEntity);

        return new StuffLocationModel {
            Location = newLocation,
            Stuff = stuff,
            Count = newEntity.Count,
        };
    }

    [HttpDelete("{stuffHash}/{locationHash}")]
    public async Task<IActionResult> Delete(string stuffHash, string locationHash)
    {
        var stuffId = StuffHasher.Decode(stuffHash);
        var locationId = LocationHasher.Decode(locationHash);
        var entity = await Context.StuffLocations.Where(x => x.StuffId == stuffId && x.LocationId == locationId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}