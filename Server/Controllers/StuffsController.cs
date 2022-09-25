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
[ApiController]
[Authorize]
public class StuffsController : BaseController<Stuff>
{
    private IStuffIdentifier StuffId { get; }

    public StuffsController(ApplicationDbContext context, IMapper mapper, IStuffIdentifier stuffId) : base(context, mapper)
    {
        StuffId = stuffId;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<StuffModel>>> Get([FromQuery] GridQuery? grid)
    {
        var query = Query;

        grid ??= new GridQuery();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.Name.ToLower().Contains(grid.Search.ToLower()));
        var count = await query.CountAsync();

        if (!string.IsNullOrEmpty(grid.SortField))
        {
            switch (grid.SortField)
            {
                case "name":
                    query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                break;
                default:
                    query = query.OrderByDescending(x => x.Created);
                    break;
            }
        }

        var list = await query
            .Skip(grid.Skip).Take(grid.Take)
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffModel>(count, list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StuffModel?>> Get(string id)
    {
        int actualId = StuffId.Decode(id);
        var query = Query.Where(x => x.Id == actualId);

        var model = await query
            .ProjectTo<StuffModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPost]
    public async Task<ActionResult<StuffModel>> Create([FromBody] StuffCreateModel model)
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

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StuffModel>> Update(string id, [FromBody] StuffCreateModel model)
    {
        if (!ModelState.IsValid || model.Name == null)
            return BadRequest(model);

        int actualId = StuffId.Decode(id);
        var slug = model.Name.ToSlug();

        var exists = await Query.AnyAsync(x => x.Id != actualId && x.Slug == slug);
        if (exists)
            return BadRequest("Account name already exists.");

        var entity = await Query.Where(x => x.Id == actualId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(model, entity);
        entity.Slug = slug;
        Audit(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<StuffModel>> Delete(string id)
    {
        int actualId = StuffId.Decode(id);
        var entity = await Query.Where(x => x.Id == actualId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<StuffModel>(entity);
    }    
}