using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Locations)]
[ApiController, Authorize]
public class LocationsController : BaseController<Location>
{
    private ILocationIdentifier LocationId { get; }

    public LocationsController(ApplicationDbContext context, IMapper mapper, ILocationIdentifier locationId) : base(context, mapper)
    {
        LocationId = locationId;
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
        int id = LocationId.Decode(hash);
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
    public Task<List<LocationBasicModel>> GetLookup() => Query
        .ProjectTo<LocationBasicModel>(Mapper.ConfigurationProvider).ToListAsync();

    [HttpGet("{hash}")]
    public async Task<ActionResult<LocationModel?>> Get(string hash)
    {
        int id = LocationId.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpGet(ApiRoutes.LocationSlug + "/{slug}")]
    public async Task<ActionResult<LocationModel?>> GetLocationBySlug(string slug)
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
    public async Task<ActionResult<LocationModel>> Create([FromBody] LocationCreateModel model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        var slug = model.Name.ToSlug();
        var exists = await Query.AnyAsync(x => x.Slug == slug);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<Location>(model);
        entity.Slug = slug;
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<LocationModel>(entity);
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<LocationModel>> Update(string hash, [FromBody] LocationCreateModel model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        int id = LocationId.Decode(hash);
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


    [HttpPut(ApiRoutes.LocationMap)]
    public async Task<IActionResult> MapPaths()
    {
        var query = Query.Include(x => x.Parent);
        foreach (var item in query)
        {
            var parent = item.Parent;
            if (parent == null)
                item.Data = new LocationData { Path = new List<LocationBasicModel>() };
            else if (parent.Data != null)
            {
                var path = parent.Data.Path?.ToList() ?? new List<LocationBasicModel>();
                path.Add(Mapper.Map<LocationBasicModel>(parent));
                item.Data = new LocationData { Path = path };
            }
        }

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<LocationModel>> Delete(string id)
    {
        int actualId = LocationId.Decode(id);
        var entity = await Query.Where(x => x.Id == actualId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<LocationModel>(entity);
    }

}