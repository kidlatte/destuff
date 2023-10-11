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
using OpenGraphNet;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Stuffs)]
[ApiController, Authorize]
public class StuffsController : BaseController<Stuff, StuffModel, StuffRequest>
{
    private IIdentityHasher<Location> LocationId { get; }

    public StuffsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Stuff> hasher, IIdentityHasher<Location> locationId) : base(context, mapper, hasher)
    {
        LocationId = locationId;
    }

    [HttpGet]
    public async Task<PagedList<StuffListItem>> Get([FromQuery] ListRequest? request)
    {
        var query = Query;

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.Name.ToLower().Contains(search) ||
                    x.Url!.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" => 
                query.OrderByDescending(x => x.Created),
            nameof(StuffListItem.Locations) => 
                request.SortDir == SortDirection.Descending ? 
                    query.OrderByDescending(x => x.Locations!.First().Name) : 
                    query.OrderBy(x => x.Locations!.First().Name),
            _ => 
                request.SortDir == SortDirection.Descending ? 
                    query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffListItem>(count, list);
    }

    [HttpGet(ApiRoutes.StuffsBySupplier + "/{supplierHash}")]
    public async Task<PagedList<StuffBasicModel>> GetBySupplier(string supplierHash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Supplier> hasher)
    {
        var supplierId = hasher.Decode(supplierHash);

        var query = Context.Suppliers.Where(x => x.Id == supplierId)
            .SelectMany(x => x.Purchases).SelectMany(x => x.Items!)
            .Select(x => x.Stuff!);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search) ||
                x.Url!.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Created),
            _ => request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffBasicModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffBasicModel>(count, list);
    }

    [HttpGet(ApiRoutes.StuffForInventory)]
    public async Task<StuffModel?> GetForInventory(int? attempts)
    {
        if (attempts == 0)
            return null;

        var random = new Random(Guid.NewGuid().GetHashCode());
        var cutoff = random.Next(40, 80);

        var query = Context.Stuffs
            //.Where(x => x.Created < DateTime.UtcNow.AddDays(-1))
            .Where(x => x.Inventoried == null || x.Inventoried < DateTime.UtcNow.AddDays(-cutoff));

        query = query.OrderBy(x => x.Events!.Count());

        var count = await query.CountAsync();
        if (count == 0)
            return await GetForInventory((attempts ?? 5) - 1);

        var limit = Math.Min(count, 25);
        int offset = random.Next(limit) % 20;

        var model = await query.Skip(offset)
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return model;
    }

    [HttpGet(ApiRoutes.StuffBySlug + "/{slug}")]
    public async Task<ActionResult<StuffModel?>> GetBySlug(string slug)
    {
        var query = Query.Where(x => x.Slug == slug);

        var model = await query
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPut(ApiRoutes.StuffScrapeUrl + "/{hash}/{command?}")]
    public async Task<IActionResult> ScrapeUrl(string hash, string? command)
    {
        var id = Hasher.Decode(hash);

        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        if (command != "refresh" && entity.Data?.OpenGraph != null || string.IsNullOrEmpty(entity.Url))
            return Ok(entity.Data?.OpenGraph);

        OpenGraph graph;

        try {
            graph = await OpenGraph.ParseUrlAsync(entity.Url);
        }
        catch (Exception) {
            return NoContent();
        }

        if (string.IsNullOrEmpty(graph.Title))
            return NoContent();

        entity.Data ??= new();
        entity.Data.OpenGraph ??= new() {
            Title = graph.Title,
            Description = graph.Metadata["description"].FirstOrDefault()?.Value,
            ImageUrl = graph.Image?.OriginalString
        };

        await Context.SaveChangesAsync();

        return Ok(entity.Data?.OpenGraph);
    }

    internal override Task BeforeCreateAsync(Stuff entity, StuffRequest request)
    {
        if (request.LocationId != null) {
            int locationId = LocationId.Decode(request.LocationId) ?? throw new NullReferenceException("LocationId");
            var stuffLocation = new StuffLocation { LocationId = locationId, Count = 1 };
            entity.StuffLocations = new List<StuffLocation> { stuffLocation };

            entity.Inventoried = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    internal override async Task AfterCreateAsync(Stuff entity)
    {
        if (entity.StuffLocations != null && entity.StuffLocations.Any()) {
            await CreateMovedEvent(entity.StuffLocations.First());
            await Context.SaveChangesAsync();
        }
    }

    internal override async Task BeforeUpdateAsync(Stuff entity, StuffRequest request)
    {
        var stuffLocations = await Context.StuffLocations
            .Where(x => x.StuffId == entity.Id).ToListAsync();
        var count = stuffLocations.Count;
        var isSingleLocation = count == 0 || count == 1 && stuffLocations.First().Count == 1;

        if (isSingleLocation) {
            if (request.LocationId != null) {
                int locationId = LocationId.Decode(request.LocationId) ?? throw new NullReferenceException("LocationId");

                if (count == 0) {
                    var stuffLocation = new StuffLocation { StuffId = entity.Id, LocationId = locationId, Count = 1 };
                    Context.Add(stuffLocation);
                    await CreateMovedEvent(stuffLocation);

                    entity.Inventoried = DateTime.UtcNow;
                }
                else if (locationId != stuffLocations.First().LocationId) {
                    Context.Remove(stuffLocations.First());

                    var stuffLocation = new StuffLocation {
                        StuffId = entity.Id,
                        LocationId = locationId,
                        Count = 1
                    };
                    Context.Add(stuffLocation);
                    await CreateMovedEvent(stuffLocation, stuffLocations.First());

                    entity.Inventoried = DateTime.UtcNow;
                }
            }
            else if (count == 1) { 
                Context.StuffLocations.RemoveRange(stuffLocations);
            }
        }
    }


    private async Task<StuffLocationModel> CreateMovedEvent(StuffLocation newEntity, StuffLocation? oldEntity = null)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == newEntity.StuffId)
            .ProjectTo<StuffBasicModel>(Mapper.ConfigurationProvider).FirstAsync();

        var query = oldEntity == null ? Context.Locations.Where(x => x.Id == newEntity.LocationId) :
            Context.Locations.Where(x => x.Id == oldEntity.LocationId || x.Id == newEntity.LocationId);

        var locations = await query.ToDictionaryAsync(x => x.Id, x => x);

        var newLocation = Mapper.Map<LocationListItem>(locations[newEntity.LocationId]);
        var oldLocation = oldEntity == null ? null :
            Mapper.Map<LocationListItem>(locations[oldEntity.LocationId]);

        var summary = oldLocation == null ? $"Set location to {newLocation.Name}" :
            $"Moved from {oldLocation.Name} to {newLocation.Name}.";

        var eventEntity = new Event {
            Type = EventType.Moved,
            StuffId = newEntity.StuffId,
            Count = newEntity.Count,
            DateTime = DateTime.UtcNow,
            Summary = summary,
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
}