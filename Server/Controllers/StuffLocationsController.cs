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

[Route(ApiRoutes.StuffLocations)]
[ApiController, Authorize]
public class StuffLocationsController : BaseController
{
    private ILocationIdentifier LocationId { get; }
    private IStuffIdentifier StuffId { get; }

    public StuffLocationsController(ApplicationDbContext context, IMapper mapper, IStuffIdentifier stuffId, ILocationIdentifier locationId) : base(context, mapper)
    {
        StuffId = stuffId;
        LocationId = locationId;
    }

    [HttpGet(ApiRoutes.StuffLocations + "/{stuffHash}")]
    public Task<List<StuffLocationModel>> Get(string? stuffHash)
    {
        var query = Context.StuffLocations.AsQueryable();

        if (stuffHash != null)
        {
            var stuffId = StuffId.Decode(stuffHash);
            query = query.Where(x => x.StuffId == stuffId);
        }

        return query.Take(20)
            .ProjectTo<StuffLocationModel>(Mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<StuffLocationModel>> Create([FromBody] StuffLocationRequest model)
    {
        if (!ModelState.IsValid || model.StuffId == null || model.LocationId == null)
            return BadRequest(model);

        var stuffId = StuffId.Decode(model.StuffId);
        var locationId = LocationId.Decode(model.LocationId);

        var exists = await Context.StuffLocations.AnyAsync(x => x.StuffId == stuffId && x.LocationId == locationId);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<StuffLocation>(model);
        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffLocationModel>(entity);
    }

    [HttpDelete("{stuffHash}/{locationHash}")]
    public async Task<IActionResult> Delete(string stuffHash, string locationHash)
    {
        int stuffId = StuffId.Decode(stuffHash);
        int locationId = LocationId.Decode(locationHash);
        var entity = await Context.StuffLocations.Where(x => x.StuffId == stuffId && x.LocationId == locationId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}