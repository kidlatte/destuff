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
    public async Task<ActionResult<PagedList<PurchaseListItem>>> Get([FromQuery] ListRequest? request)
    {
        var query = Query;

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Supplier!.ShortName.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search) ||
                x.Supplier!.Name.ToLower().Contains(search));
        }

        switch (request.SortField)
        {
            case nameof(PurchaseListItem.Received):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Received ?? x.Receipt) : query.OrderBy(x => x.Received ?? x.Receipt);
                break;
            case nameof(PurchaseListItem.Supplier):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Supplier!.ShortName) : query.OrderBy(x => x.Supplier!.ShortName);
                break;
            case nameof(PurchaseListItem.Price):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                break;
            default:
                query = query.OrderByDescending(x => x.Received == null && x.Receipt == null)
                    .ThenByDescending(x => x.Received ?? x.Receipt);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
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
    public async Task<ActionResult<PurchaseModel>> Create([FromBody] PurchaseRequest model)
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
    public async Task<ActionResult<PurchaseModel>> Update(string hash, [FromBody] PurchaseRequest model)
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