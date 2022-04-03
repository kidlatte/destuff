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
[ApiController]
[Authorize]
public class LocationsController : BaseController<Location>
{
    private ILocationIdService _locationId { get; }

    public LocationsController(ApplicationDbContext context, IMapper mapper, ILocationIdService locationId): base(context, mapper)
    {
        _locationId = locationId;
    }

    // [HttpGet("{parentHash}")]
    // public async Task<ActionResult<List<LocationModel>>> GetLocations(string? parentHash)
    // {
    //     int? parentId = parentHash != null ? _locationId.Decode(parentHash) : default;
    //     var query = Query.Where(x => x.ParentId == parentId);

    //     return await query
    //         .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
    //         .ToListAsync();
    // }

    [HttpGet]
    public async Task<ActionResult<List<LocationModel>>> GetLocations()
    {
        var query = Query.Where(x => x.ParentId == null);

        return await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<LocationModel>> CreateLocation([FromBody] LocationCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        var entity = Mapper.Map<Location>(model);
        Audit(entity);
        Context.Add(entity);

        await Context.SaveChangesAsync();
        return Mapper.Map<LocationModel>(entity);
    }

}