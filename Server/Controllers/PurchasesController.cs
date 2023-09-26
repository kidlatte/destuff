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

[Route(ApiRoutes.Purchases)]
[ApiController, Authorize]
public class PurchasesController : BaseController<Purchase, PurchaseModel, PurchaseRequest>
{
    public PurchasesController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Purchase> hasher) : base(context, mapper, hasher)
    {
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

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Received == null && x.Receipt == null).ThenByDescending(x => x.Received ?? x.Receipt),
            nameof(PurchaseListItem.Received) => request.SortDir == SortDirection.Descending ? 
                query.OrderByDescending(x => x.Received ?? x.Receipt) : query.OrderBy(x => x.Received ?? x.Receipt),
            nameof(PurchaseListItem.Supplier) => request.SortDir == SortDirection.Descending ? 
                query.OrderByDescending(x => x.Supplier!.ShortName) : query.OrderBy(x => x.Supplier!.ShortName),
            _ => request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };
        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseListItem>(count, list);
    }

    [HttpGet(ApiRoutes.PurchasesBySupplier + "/{hash}")]
    public async Task<PagedList<PurchaseBasicModel>> GetBySupplier(string hash, [FromQuery] ListRequest? request, [FromServices] IIdentityHasher<Supplier> hasher)
    {
        var id = hasher.Decode(hash);
        var query = Query.Where(x => x.SupplierId == id);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Supplier!.ShortName.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search) ||
                x.Supplier!.Name.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Received == null && x.Receipt == null).ThenByDescending(x => x.Received ?? x.Receipt),
            nameof(PurchaseBasicModel.Received) => request.SortDir == SortDirection.Descending ? 
                query.OrderByDescending(x => x.Received ?? x.Receipt) : query.OrderBy(x => x.Received ?? x.Receipt),
            _ => request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseBasicModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseBasicModel>(count, list);
    }

    internal override async Task BeforeUpdateAsync(Purchase entity, PurchaseRequest _)
    {
        var items = await Context.PurchaseItems.Where(x => x.PurchaseId == entity.Id)
            .Select(x => new PurchaseItem() { Id = x.Id, DateTime = x.DateTime, Quantity = x.Quantity, Price = x.Price })
            .ToListAsync();

        entity.ItemCount = items.Count;
        entity.Price = items.Sum(x => x.Quantity * x.Price);

        foreach (var item in items) {
            Context.Attach(item);
            item.DateTime = entity.Received ?? entity.Receipt ?? entity.Created;
        }
    }

    internal override async Task AfterSaveAsync(Purchase entity) => await CountSupplierPurchases(entity.SupplierId);

    internal override async Task AfterDeleteAsync(Purchase entity) => await CountSupplierPurchases(entity.SupplierId);

    private async Task CountSupplierPurchases(int? supplierId)
    {
        if (supplierId == null)
            return;

        var supplier = await Context.Suppliers.Where(x => x.Id == supplierId).FirstAsync();
        supplier.PurchaseCount = await Context.Purchases
            .Where(x => x.SupplierId == supplierId).CountAsync();

        await Context.SaveChangesAsync();
    }
}