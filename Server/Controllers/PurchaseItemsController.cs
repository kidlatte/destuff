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
using System.Linq;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.PurchaseItems)]
[ApiController, Authorize]
public class PurchaseItemsController : BaseController<PurchaseItem, PurchaseItemModel, PurchaseItemRequest>
{
    public PurchaseItemsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<PurchaseItem> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpGet]
    public async Task<PagedList<PurchaseItemListItem>> Get([FromQuery(Name = "pid")] string purchaseHash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Purchase> hasher)
    {
        var purchaseId = hasher.Decode(purchaseHash);
        var query = Query.Where(x => x.PurchaseId == purchaseId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
        }

        switch (request.SortField)
        {
            case nameof(PurchaseItemListItem.Stuff):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Stuff) : query.OrderBy(x => x.Stuff);
                break;
            case nameof(PurchaseItemListItem.Quantity):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Quantity) : query.OrderBy(x => x.Quantity);
                break;
            case nameof(PurchaseItemListItem.Price):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price);
                break;
            default:
                query = query.OrderByDescending(x => x.Created);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseItemListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseItemListItem>(count, list);
    }

    [HttpGet(ApiRoutes.PurchaseItemsByStuff + "/{stuffHash}")]
    public async Task<PagedList<PurchaseItemSupplier>> GetByStuff(string stuffHash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Stuff> hasher)
    {
        int stuffId = hasher.Decode(stuffHash);
        var query = Query.Where(x => x.StuffId == stuffId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
        }
        var count = await query.CountAsync();

        var sortField = request.SortField ?? "";
        switch (sortField)
        {
            case "":
                query = query.OrderByDescending(x => x.Created);
                break;
            case $"({nameof(PurchaseItemSupplier.Purchase)}.{nameof(PurchaseItemSupplier.Purchase.Supplier)})":
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Purchase!.Supplier!.Name) : query.OrderBy(x => x.Purchase!.Supplier!.Name);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseItemSupplier>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseItemSupplier>(count, list);
    }

    internal override async Task AfterSaveAsync(PurchaseItem entity) 
        => await ComputePurchasePrice(entity.PurchaseId);

    internal override async Task AfterDeleteAsync(PurchaseItem entity) 
        => await ComputePurchasePrice(entity.PurchaseId);

    private async Task ComputePurchasePrice(int purchaseId)
    {
        var result = await Context.Purchases.Where(x => x.Id == purchaseId).Select(x => new
        {
            x.Price,
            Computed = x.Items!.Sum(x => x.Quantity * x.Price)
        }).FirstOrDefaultAsync();

        if (result != null && result.Price != result.Computed)
        {
            var purchase = new Purchase { Id = purchaseId, Price = result.Price, };
            Context.Attach(purchase);
            purchase.Price = result.Computed;
            await Context.SaveChangesAsync();
        }
    }
}