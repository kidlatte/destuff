using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Locations)]
[ApiController]
[Authorize]
public class LocationsController : BaseController<Location>
{
    public LocationsController(ApplicationDbContext context, IMapper mapper): base(context, mapper)
    {
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