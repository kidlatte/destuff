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

[Route(ApiRoutes.Stuffs)]
[ApiController]
[Authorize]
public class StuffsController : BaseController<Stuff>
{
    private IStuffIdentifier StuffId { get; }

    public StuffsController(ApplicationDbContext context, IMapper mapper, IStuffIdentifier stuffId) : base(context, mapper)
    {
        StuffId = stuffId;
    }

    [HttpPost]
    public async Task<ActionResult<StuffModel>> Create([FromBody] StuffCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        var slug = model.Name!.ToSlug();
        var exists = await Query.AnyAsync(x => x.Slug == slug);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<Stuff>(model);
        entity.Slug = slug;
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }
}