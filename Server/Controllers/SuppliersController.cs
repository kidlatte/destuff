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
    private IIdentityHasher<Supplier> Hasher { get; }

    public SuppliersController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Supplier> hasher) : base(context, mapper)
    {
        Hasher = hasher;
    }

    [HttpGet]
    public async Task<PagedList<SupplierListItem>> Get([FromQuery] ListRequest? request)
    {
        var query = Query;

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.ShortName.ToLower().Contains(search) ||
                x.Name.ToLower().Contains(search) ||
                x.Url!.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        switch (sortField)
        {
            case "":
                query = query.OrderByDescending(x => x.Created);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<SupplierListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<SupplierListItem>(count, list);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<SupplierModel?>> Get(string hash)
    {
        int id = Hasher.Decode(hash);
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
    public async Task<ActionResult<SupplierModel>> Create([FromBody] SupplierRequest model)
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
    public async Task<ActionResult<SupplierModel>> Update(string hash, [FromBody] SupplierRequest model)
    {
        if (!ModelState.IsValid || model.ShortName == null)
            return BadRequest(model);

        int id = Hasher.Decode(hash);
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
        int id = Hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}