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

    [HttpGet]
    public async Task<ActionResult<List<LocationModel>>> GetLocations()
    {
        var query = Query.Where(x => x.ParentId == null);

        return await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationModel?>> GetLocation(string id)
    {
        int actualId = _locationId.Decode(id);
        var query = Query.Where(x => x.Id == actualId);

        return await query
            .ProjectTo<LocationModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
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