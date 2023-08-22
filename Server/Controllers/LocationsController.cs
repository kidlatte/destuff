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

[Route(ApiRoutes.Locations)]
[ApiController, Authorize]
public class LocationsController : BaseController<Location, LocationModel, LocationRequest>
{
    public LocationsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Location> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IList<LocationModel>>> Get()
    {
        var query = Query;

        var list = await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        // assemble tree
        var result = list.Select(x => x).ToList();
        list.ForEach(item => 
        {
            if (item.ParentId != null)
            {
                var parent = list.First(x => x.Id == item.ParentId);
                if (parent != null)
                {
                    parent.Children = (parent.Children ?? new List<LocationModel>()).Append(item).ToList();
                    result.Remove(item);
                }
            }
        });

        return result;
    }

    [HttpGet(ApiRoutes.LocationTree + "/{hash}")]
    public async Task<ActionResult<LocationTreeModel?>> GetLocationTree(string hash)
    {
        var id = Hasher.Decode(hash);
        var query = Query.Include(x => x.Children).Where(x => x.Id == id);

        var model = await query
            .ProjectTo<LocationTreeModel?>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        // TODO: use supporting hierarchy table
        if (model.Children != null)
        foreach (var item in model.Children)
        {
            var _item = (await GetLocationTree(item.Id!)).Value;
            item.Children = _item?.Children;
        }

        return model;
    }

    [Route(ApiRoutes.LocationLookup)]
    [HttpGet]
    public async Task<PagedList<LocationLookupItem>> GetLookup([FromQuery] ListRequest? request)
    {
        var query = Query;

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.Name.ToLower().Contains(search)));
        }
            

        var desc = SortDirection.Descending;
        switch (request.SortField)
        {
            case nameof(Location.Id):
                query = request.SortDir == desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                break;
            case "name":
                query = request.SortDir == desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<LocationLookupItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<LocationLookupItem>(count, list);
    }

    [HttpGet(ApiRoutes.LocationSlug + "/{slug}")]
    public async Task<ActionResult<LocationModel>> GetLocationBySlug(string slug)
    {
        var query = Query.Where(x => x.Slug == slug);

        var model = await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPut(ApiRoutes.LocationMap)]
    public async Task<IActionResult> MapPaths()
    {
        var query = Query.Include(x => x.Parent);
        foreach (var item in query)
        {
            var parent = item.Parent;
            if (parent == null)
                item.Data = new LocationData { Path = new List<LocationListItem>() };
            else if (parent.Data != null)
            {
                var path = parent.Data.Path?.ToList() ?? new List<LocationListItem>();
                path.Add(Mapper.Map<LocationListItem>(parent));
                item.Data = new LocationData { Path = path };
            }
        }

        await Context.SaveChangesAsync();

        return NoContent();
    }

    internal override async Task BeforeSaveAsync(Location entity)
    {
        entity.Data = await GenerateData(entity.ParentId);
        await GenerateChildrenData(entity);
    }

    private async Task<LocationData> GenerateData(int? parentId)
    {
        if (parentId == null)
            return new LocationData { Path = new List<LocationListItem>() };

        var parent = await Context.Locations.Where(x => x.Id == parentId).FirstOrDefaultAsync();
        return GenerateData(parent);
    }

    private LocationData GenerateData(Location? parent)
    {
        if (parent == null)
            return new LocationData { Path = new List<LocationListItem>() };

        if (parent.Data == null)
            return new LocationData { Path = new[] { Mapper.Map<LocationListItem>(parent) } };

        var path = parent.Data.Path?.ToList() ?? new List<LocationListItem>();
        path.Add(Mapper.Map<LocationListItem>(parent));

        return new LocationData { Path = path };
    }

    private async Task GenerateChildrenData(Location parent)
    {
        var children = await Context.Locations.Where(x => x.ParentId == parent.Id).ToListAsync();
        foreach (var child in children) 
        {
            child.Data = GenerateData(parent);
            await GenerateChildrenData(child);
        }
    }
}