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
using BlazorGrid.Abstractions;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Purchases)]
[ApiController, Authorize]
public class PurchasesController : BaseController<Purchase>
{
    private IPurchaseIdentifier PurchaseId { get; }

    public PurchasesController(ApplicationDbContext context, IMapper mapper, IPurchaseIdentifier purchaseId) : base(context, mapper)
    {
        PurchaseId = purchaseId;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<PurchaseListItem>>> Get([FromQuery] GridQuery? grid)
    {
        var query = Query;

        grid ??= new GridQuery();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.Supplier!.ShortName.ToLower().Contains(grid.Search.ToLower()) ||
                x.Supplier!.Name.ToLower().Contains(grid.Search.ToLower()));

        switch (grid.SortField)
        {
            case "receipt":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Receipt) : query.OrderBy(x => x.Receipt);
                break;
            case "received":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Received) : query.OrderBy(x => x.Received);
                break;
            case "supplier":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Supplier!.ShortName) : query.OrderBy(x => x.Supplier!.ShortName);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(grid.Skip).Take(grid.Take)
            .ProjectTo<PurchaseListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseListItem>(count, list);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<PurchaseModel?>> Get(string hash)
    {
        int id = PurchaseId.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<PurchaseModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseModel>> Create([FromBody] PurchaseCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        var entity = Mapper.Map<Purchase>(model);
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<PurchaseModel>(entity);
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<PurchaseModel>> Update(string hash, [FromBody] PurchaseCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        int id = PurchaseId.Decode(hash);

        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(model, entity);
        Audit(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<PurchaseModel>(entity);
    }

    [HttpDelete("{hash}")]
    public async Task<IActionResult> Delete(string hash)
    {
        int id = PurchaseId.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}