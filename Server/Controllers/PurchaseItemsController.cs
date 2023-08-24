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

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Created),
            nameof(PurchaseItemListItem.Stuff) => request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Stuff!.Name) : query.OrderBy(x => x.Stuff!.Name),
            _ => request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

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
        var stuffId = hasher.Decode(stuffHash);
        var query = Query.Where(x => x.StuffId == stuffId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searches = request.Search.ToLower().Split(" ").ToList();
            searches.ForEach(search =>
                query = query.Where(x => x.Stuff!.Name.ToLower().Contains(search) ||
                    x.Notes!.ToLower().Contains(search)));
        }

        var sortField = request.SortField ?? "";
        var desc = request.SortDir == SortDirection.Descending;
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Created),
            $"({nameof(PurchaseItemSupplier.Purchase)}.{nameof(PurchaseItemSupplier.Purchase.Supplier)})" 
                => request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Purchase!.Supplier!.Name) : query.OrderBy(x => x.Purchase!.Supplier!.Name),
            _ => desc ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseItemSupplier>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseItemSupplier>(count, list);
    }

    internal override async Task BeforeSaveAsync(PurchaseItem entity)
    {
        var purchase = await Context.Purchases.Where(x => x.Id == entity.PurchaseId)
            .Select(x => new
        {
            x.SupplierId,
            x.Received,
            x.Receipt,
            x.Created
        }).FirstOrDefaultAsync();

        if (purchase != null) 
            entity.DateTime = purchase.Received ?? purchase.Receipt ?? purchase.Created;

        entity.Count = entity.Quantity;
        entity.Data = await GenerateData(entity, purchase?.SupplierId);
    }

    private async Task<EventData> GenerateData(PurchaseItem entity, int? supplierId)
    {
        var stuff = await Context.Stuffs.Where(x => x.Id == entity.StuffId)
            .ProjectTo<StuffBasicModel>(Mapper.ConfigurationProvider).FirstAsync();

        var supplier = default(SupplierBasicModel);
        if (supplierId != null)
            supplier = await Context.Suppliers.Where(x => x.Id == supplierId)
                .ProjectTo<SupplierBasicModel>(Mapper.ConfigurationProvider).FirstAsync();

        return new EventData {
            Difference = entity.Quantity,
            PurchaseItem = Mapper.Map<PurchaseItemBasicModel>(entity),
            Stuff = stuff,
            Supplier = supplier,
        };
    }

    internal override Task AfterSaveAsync(PurchaseItem entity) 
        => ComputePurchaseProperties(entity.PurchaseId);

    internal override Task AfterDeleteAsync(PurchaseItem entity) 
        => ComputePurchaseProperties(entity.PurchaseId);

    private async Task ComputePurchaseProperties(int purchaseId)
    {
        var result = await Context.Purchases.Where(x => x.Id == purchaseId).Select(x => new
        {
            x.ItemCount,
            NewItemCount = x.Items!.Count(),
            x.Price,
            NewPrice = x.Items!.Sum(x => x.Quantity * x.Price),
        }).FirstOrDefaultAsync();

        if (result != null)
        {
            var purchase = new Purchase { Id = purchaseId, Price = result.Price, ItemCount = result.ItemCount };
            Context.Attach(purchase);
            purchase.ItemCount = result.NewItemCount;
            purchase.Price = result.NewPrice;
            await Context.SaveChangesAsync();
        }
    }
}