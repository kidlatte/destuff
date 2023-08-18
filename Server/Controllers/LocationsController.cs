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
public class LocationsController : BaseController<Location>
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
        int id = Hasher.Decode(hash);
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
    public async Task<PagedList<LocationLookupItem>> GetLookup([FromQuery] ListRequest? grid)
    {
        var query = Query;

        grid ??= new ListRequest();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.Name.ToLower().Contains(grid.Search.ToLower()));

        var desc = SortDirection.Descending;
        switch (grid.SortField)
        {
            case nameof(Location.Id):
                query = grid.SortDir == desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                break;
            case "name":
                query = grid.SortDir == desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(grid.Skip).Take(grid.Take)
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

    [HttpPost]
    public async Task<ActionResult<LocationModel>> Create([FromBody] LocationRequest model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        var slug = model.Name.ToSlug();
        var exists = await Query.AnyAsync(x => x.Slug == slug);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<Location>(model);
        entity.Slug = slug;
        entity.Data = await GenerateData(entity.ParentId);
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<LocationModel>(entity);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<LocationModel?>> Get(string hash)
    {
        int id = Hasher.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<LocationModel>> Update(string hash, [FromBody] LocationRequest model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        int id = Hasher.Decode(hash);
        var slug = model.Name.ToSlug();

        var exists = await Query.AnyAsync(x => x.Id != id && x.Slug == slug);
        if (exists)
            return BadRequest("Account name already exists.");

        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(model, entity);
        entity.Slug = slug;
        Audit(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<LocationModel>(entity);
    }

    private async Task<LocationData> GenerateData(int? parentId)
    {
        if (parentId == null)
            return new LocationData { Path = new List<LocationListItem>() };

        var parent = await Context.Locations.Where(x => x.Id == parentId).FirstOrDefaultAsync();
        if (parent == null)
            return new LocationData { Path = new List<LocationListItem>() };

        if (parent.Data == null)
            return new LocationData { Path = new[] { Mapper.Map<LocationListItem>(parent) } };

        var path = parent.Data.Path?.ToList() ?? new List<LocationListItem>();
        path.Add(Mapper.Map<LocationListItem>(parent));
        return new LocationData { Path = path };
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
}