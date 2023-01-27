using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.PurchaseItems)]
[ApiController, Authorize]
public class PurchaseItemsController : BaseController<PurchaseItem>
{
    private IPurchaseIdentifier PurchaseId { get; }
    private IPurchaseItemIdentifier PurchaseItemId { get; }

    public PurchaseItemsController(ApplicationDbContext context, IMapper mapper, IPurchaseIdentifier purchaseId, IPurchaseItemIdentifier supplierId) : base(context, mapper)
    {
        PurchaseId = purchaseId;
        PurchaseItemId = supplierId;
    }

    [HttpGet]
    public async Task<PagedList<PurchaseItemListItem>> Get([FromQuery(Name = "pid")] string purchaseId, [FromQuery] GridQuery? grid)
    {
        var pid = PurchaseId.Decode(purchaseId);
        var query = Query.Where(x => x.PurchaseId == pid);

        grid ??= new GridQuery();
        if (!string.IsNullOrEmpty(grid.Search))
            query = query.Where(x => x.Stuff!.Name.ToLower().Contains(grid.Search.ToLower()));

        switch (grid.SortField)
        {
            case "quantity":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Quantity) : query.OrderBy(x => x.Quantity);
                break;
            case "cost":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                break;
            case "stuff":
                query = grid.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Stuff) : query.OrderBy(x => x.Stuff);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(grid.Skip).Take(grid.Take)
            .ProjectTo<PurchaseItemListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseItemListItem>(count, list);
    }

    [HttpGet("{hash}")]
    public async Task<ActionResult<PurchaseItemModel?>> Get(string hash)
    {
        int id = PurchaseItemId.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<PurchaseItemModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseItemModel>> Create([FromBody] PurchaseItemCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        var entity = Mapper.Map<PurchaseItem>(model);
        Audit(entity);

        Context.Add(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<PurchaseItemModel>(entity);
    }

    [HttpPut("{hash}")]
    public async Task<ActionResult<PurchaseItemModel>> Update(string hash, [FromBody] PurchaseItemCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(model);

        int id = PurchaseItemId.Decode(hash);

        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(model, entity);
        Audit(entity);
        await Context.SaveChangesAsync();

        return Mapper.Map<PurchaseItemModel>(entity);
    }

    [HttpDelete("{hash}")]
    public async Task<IActionResult> Delete(string hash)
    {
        int id = PurchaseItemId.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }    
}