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

    [HttpPut]
    public async Task<ActionResult<StuffLocationModel>> Update([FromBody] StuffLocationRequest model)
    {
        if (!ModelState.IsValid || model.StuffId == null || model.LocationId == null)
            return BadRequest(model);

        var stuffId = StuffHasher.Decode(model.StuffId);
        var locationId = LocationHasher.Decode(model.LocationId);

        var entity = await Context.StuffLocations.FirstOrDefaultAsync(x => x.StuffId == stuffId && x.LocationId == locationId);
        if (entity == null)
            return BadRequest("Location does not exist.");

        Mapper.Map(model, entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffLocationModel>(entity);
    }

    [HttpDelete("{stuffHash}/{locationHash}")]
    public async Task<IActionResult> Delete(string stuffHash, string locationHash)
    {
        int stuffId = StuffHasher.Decode(stuffHash);
        int locationId = LocationHasher.Decode(locationHash);
        var entity = await Context.StuffLocations.Where(x => x.StuffId == stuffId && x.LocationId == locationId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}