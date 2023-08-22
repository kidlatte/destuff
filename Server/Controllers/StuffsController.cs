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
            var search = request.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search) ||
                x.Url!.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
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

    internal override Task BeforeSaveAsync(Stuff entity, StuffRequest request)
    {
        var count = entity.StuffLocations?.Count ?? 0;
        var isSingleLocation = count == 0 || count == 1 && entity.StuffLocations?.First().Count == 1;
        if (isSingleLocation)
        {
            if (request.LocationId == null)
            {
                entity.StuffLocations = new List<StuffLocation>();
            }
            else
            {
                int locationId = LocationId.Decode(request.LocationId) ?? throw new NullReferenceException("LocationId");
                var stuffLocation = new StuffLocation { LocationId = locationId, Count = 1 };
                entity.StuffLocations = new List<StuffLocation> { stuffLocation };
            }
        }

        return Task.CompletedTask;
    }
}