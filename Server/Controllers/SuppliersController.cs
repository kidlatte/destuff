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

[Route(ApiRoutes.Suppliers)]
[ApiController, Authorize]
public class SuppliersController : BaseController<Supplier>
{
    private ISupplierIdentifier SupplierId { get; }

    public SuppliersController(ApplicationDbContext context, IMapper mapper, ISupplierIdentifier supplierId) : base(context, mapper)
    {
        SupplierId = supplierId;
    }

    [HttpGet]
    public async Task<PagedList<SupplierListItem>> Get([FromQuery] GridQuery? grid)
    {
        var query = Query;

        grid ??= new GridQuery();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.ShortName.ToLower().Contains(grid.Search.ToLower()) ||
                x.Name.ToLower().Contains(grid.Search.ToLower()));

        switch (grid.SortField)
        {
            case "shortName":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.ShortName) : query.OrderBy(x => x.ShortName);
                break;
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
            .ProjectTo<SupplierListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<SupplierListItem>(count, list);
    }

    [Route(ApiRoutes.SupplierLookup)]
    [HttpGet]
    public Task<List<SupplierListItem>> GetLookup() => Query
        .ProjectTo<SupplierListItem>(Mapper.ConfigurationProvider).ToListAsync();

    [HttpGet("{hash}")]
    public async Task<ActionResult<SupplierModel?>> Get(string hash)
    {
        int id = SupplierId.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<SupplierModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpGet(ApiRoutes.SupplierSlug + "/{slug}")]
    public async Task<ActionResult<SupplierModel>> GetBySlug(string slug)
    {
        var query = Query.Where(x => x.Slug == slug);

        var model = await query
            .ProjectTo<SupplierModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPost]
    public async Task<ActionResult<SupplierModel>> Create([FromBody] SupplierCreateModel model)
    {
        if (!ModelState.IsValid || model.ShortName == null)
            return BadRequest(model);

        var slug = model.ShortName.ToSlug();
        var exists = await Query.AnyAsync(x => x.Slug == slug);
        if (exists)
            return BadRequest("Name already exists.");

        var entity = Mapper.Map<Supplier>(model);
        entity.Slug = slug;
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<SupplierModel>(entity);
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<SupplierModel>> Update(string hash, [FromBody] SupplierCreateModel model)
    {
        if (!ModelState.IsValid || model.ShortName == null)
            return BadRequest(model);

        int id = SupplierId.Decode(hash);
        var slug = model.ShortName.ToSlug();

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

        return Mapper.Map<SupplierModel>(entity);
    }

    [HttpDelete("{hash}")]
    public async Task<IActionResult> Delete(string hash)
    {
        int id = SupplierId.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}