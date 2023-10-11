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
    public async Task<ActionResult<IList<LocationTreeItem>>> Get()
    {
        var query = Query;

        var list = await query
            .OrderBy(x => x.Order).ThenBy(x => x.Id)
            .ProjectTo<LocationTreeItem>(Mapper.ConfigurationProvider)
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
                    parent.Children = (parent.Children ?? new List<LocationTreeItem>()).Append(item).ToList();
                    result.Remove(item);
                }
            }
        });

        return result;
    }

    [HttpGet(ApiRoutes.LocationTree + "/{hash}")]
    public async Task<ActionResult<LocationTreeItem?>> GetLocationTree(string hash)
    {
        var id = Hasher.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<LocationTreeItem>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        // TODO: use supporting hierarchy table
        model.Children = await GetChildren(model);

        return model;
    }

    async Task<ICollection<LocationTreeItem>> GetChildren(LocationTreeItem parent)
    {
        var parentId = Hasher.Decode(parent.Id);
        var query = Query.Where(x => x.ParentId == parentId);

        var children = await query
            .OrderBy(x => x.Order).ThenBy(x => x.Id)
            .ProjectTo<LocationTreeItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        foreach (var item in children) {
            item.Children = await GetChildren(item);
        }

        return children;
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
                query = query.Where(x => x.Name.ToLower().Contains(search)
                    || x.Slug.ToLower().Contains(search)));
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

    [HttpGet(ApiRoutes.LocationBySlug + "/{slug}")]
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
                item.Data = new LocationData(item.Name);
            else if (parent.Data != null)
            {
                var path = parent.Data.Path?.ToList() ?? new List<LocationListItem>();
                path.Add(Mapper.Map<LocationListItem>(parent));
                
                item.Data = new LocationData(item.Name, path);
            }
        }

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut(ApiRoutes.LocationOrderUp + "/{hash}")]
    public async Task<IActionResult> MoveOrderUp(string hash)
    {
        var id = Hasher.Decode(hash);

        var parentId = await Query.Where(x => x.Id == id)
            .Select(x => x.ParentId).FirstOrDefaultAsync();

        var siblings = await Query.Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Order).ThenBy(x => x.Id)
            .ToListAsync();

        for (int i = 0; i < siblings.Count; i++) {
            var sibling = siblings[i];

            if (i > 0 && sibling.Id == id) {
                sibling.Order = i - 1;
                siblings[i - 1].Order = i;
            }
            else {
                sibling.Order = i;
            }
        }

        await Context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut(ApiRoutes.LocationOrderDown + "/{hash}")]
    public async Task<IActionResult> MoveOrderDown(string hash)
    {
        var id = Hasher.Decode(hash);

        var parentId = await Query.Where(x => x.Id == id)
            .Select(x => x.ParentId).FirstOrDefaultAsync();

        var siblings = await Query.Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Order).ThenBy(x => x.Id)
            .ToListAsync();

        var count = siblings.Count;
        for (int i = count - 1; i >= 0; i--) {
            var sibling = siblings[i];

            if (i < count - 1 && sibling.Id == id) {
                sibling.Order = i + 1;
                siblings[i + 1].Order = i;
            }
            else {
                sibling.Order = i;
            }
        }

        await Context.SaveChangesAsync();
        return NoContent();
    }

    internal override async Task BeforeCreateAsync(Location entity, LocationRequest _)
    {
        var siblingCount = await Query.CountAsync(x => x.ParentId == entity.ParentId);
        entity.Order = siblingCount;
        await BeforeSaveAsync(entity);
    }

    internal override async Task BeforeSaveAsync(Location entity)
    {
        entity.Data = await GenerateData(entity);
        await GenerateChildrenData(entity);
    }

    private async Task<LocationData> GenerateData(Location entity)
    {
        var parent = await Context.Locations.Where(x => x.Id == entity.ParentId).FirstOrDefaultAsync();

        if (parent == null)
            return new LocationData(entity.Name);

        entity.Slug = $"{parent.Slug}-{entity.ToSlug()}";

        parent.Data ??= new LocationData(parent.Name);

        var path = parent.Data.Path?.ToList() ?? new List<LocationListItem>();
        path.Add(Mapper.Map<LocationListItem>(parent));

        return new LocationData(entity.Name, path);
    }

    private async Task GenerateChildrenData(Location parent)
    {
        var children = await Context.Locations.Where(x => x.ParentId == parent.Id).ToListAsync();
        foreach (var child in children) 
        {
            child.Data = await GenerateData(child);
            await GenerateChildrenData(child);
        }
    }
}