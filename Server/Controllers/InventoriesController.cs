using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Inventories)]
[ApiController, Authorize]
public class InventoriesController : BaseController
{
    public InventoriesController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
            
    }

    [HttpGet(ApiRoutes.InventoryStuff)]
    public async Task<StuffModel?> GetStuff(int? attempts)
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
            return await GetStuff((attempts ?? 5) - 1);

        var limit = Math.Min(count, 25);
        //int offset = random.Next(limit) % 20;
        int offset = 0;

        var model = await query.Skip(offset)
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return model;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        var entity = Mapper.Map<Event>(model);
        Context.Add(entity);
        entity.Type = EventType.Inventory;
        entity.DateTime = DateTime.UtcNow;
        Audit(entity);

        entity.Data = await GenerateData(entity);
        entity.Summary = GenerateSummary(entity.Data.Locations);

        await Context.SaveChangesAsync();
        return Ok();
    }

    private async Task<EventData> GenerateData(Event entity)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == entity.StuffId).FirstAsync();
        stuff.Inventoried = entity.DateTime;

        var locations = await Context.StuffLocations.Where(x => x.StuffId == entity.StuffId)
            .ProjectTo<StuffLocationBasicModel>(Mapper.ConfigurationProvider).ToListAsync();

        return new EventData {
            Stuff = Mapper.Map<StuffBasicModel>(stuff),
            Locations = locations
        };
    }

    private string GenerateSummary(ICollection<StuffLocationBasicModel>? locations)
    {
        if (locations == null)
            return $"No location on record";

        var count = locations.Sum(x => x.Count);
        if (count == 0)
            return $"No location on record";

        var location = locations.First().Location;
        if (count == 1)
            return $"Located in {location.Name}";

        if (locations.Count == 1)
            return $"{count} units are in {location.Name}";
            
        return $"{count} total units are in {locations.Count} locations.";
    }
}
