using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Stuffs)]
[ApiController, Authorize]
public class StuffsController : BaseController<Stuff>
{
    private IStuffIdentifier StuffId { get; }
    private ILocationIdentifier LocationId { get; }

    public StuffsController(ApplicationDbContext context, IMapper mapper, IStuffIdentifier stuffId, ILocationIdentifier locationId) : base(context, mapper)
    {
        StuffId = stuffId;
        LocationId = locationId;
    }

    [HttpGet]
    public async Task<PagedList<StuffListItem>> Get([FromQuery] GridQuery? grid)
    {
        var query = Query;

        grid ??= new GridQuery();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.Name.ToLower().Contains(grid.Search.ToLower()) ||
                x.Url!.ToLower().Contains(grid.Search.ToLower()) ||
                x.Notes!.ToLower().Contains(grid.Search.ToLower()));

        switch (grid.SortField)
        {
            case "name":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(grid.Skip).Take(grid.Take)
            .ProjectTo<StuffListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffListItem>(count, list);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<StuffModel?>> Get(string hash)
    {
        int id = StuffId.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
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

    [HttpPost]
    public async Task<ActionResult<StuffModel>> Create([FromBody] StuffRequest model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        var slug = model.Name.ToSlug();
        var exists = await Query.AnyAsync(x => x.Slug == slug);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<Stuff>(model);
        entity.Slug = slug;
        Audit(entity);

        if (model.LocationId != null)
        {
            var locationId = LocationId.Decode(model.LocationId);
            var stuffLocation = new StuffLocation { LocationId = locationId, Count = 1 };
            entity.StuffLocations = new List<StuffLocation> { stuffLocation };
        }

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<StuffModel>> Update(string hash, [FromBody] StuffRequest model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        int id = StuffId.Decode(hash);
        var slug = model.Name.ToSlug();

        var exists = await Query.AnyAsync(x => x.Id != id && x.Slug == slug);
        if (exists)
            return BadRequest("Account name already exists.");

        var entity = await Query.Include(x => x.StuffLocations!.Take(2))
            .Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(model, entity);
        entity.Slug = slug;
        Audit(entity);

        var count = entity.StuffLocations?.Count ?? 0;
        var isSingleLocation = count == 0  || count == 1 && entity.StuffLocations?.First().Count == 1;
        if (isSingleLocation)
        {
            if (model.LocationId == null)
            {
                entity.StuffLocations = new List<StuffLocation>();
            }
            else
            {
                var locationId = LocationId.Decode(model.LocationId);
                var stuffLocation = new StuffLocation { LocationId = locationId, Count = 1 };
                entity.StuffLocations = new List<StuffLocation> { stuffLocation };
            }
        }

        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }

    [HttpDelete("{hash}")]
    public async Task<IActionResult> Delete(string hash)
    {
        int id = StuffId.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}