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
        switch (sortField) {
            case "":
                query = query.OrderByDescending(x => x.Created);
                break;
            case nameof(StuffListItem.Locations):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Locations!.First().Name) : query.OrderBy(x => x.Locations!.First().Name);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffListItem>(count, list);
    }

    [HttpGet(ApiRoutes.StuffSlug + "/{slug}")]
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
                var locationId = LocationId.Decode(request.LocationId);
                var stuffLocation = new StuffLocation { LocationId = locationId, Count = 1 };
                entity.StuffLocations = new List<StuffLocation> { stuffLocation };
            }
        }

        return Task.CompletedTask;
    }
}