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
        switch (sortField) {
            case "":
                query = query.OrderByDescending(x => x.Received == null && x.Receipt == null)
                    .ThenByDescending(x => x.Received ?? x.Receipt);
                break;
            case nameof(PurchaseListItem.Received):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Received ?? x.Receipt) : query.OrderBy(x => x.Received ?? x.Receipt);
                break;
            case nameof(PurchaseListItem.Supplier):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Supplier!.ShortName) : query.OrderBy(x => x.Supplier!.ShortName);
                break;
            case nameof(PurchaseListItem.ItemCount):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Items!.Count()) : query.OrderBy(x => x.Items!.Count());
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

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
        int id = hasher.Decode(hash);
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
        switch (sortField)
        {
            case "":
                query = query.OrderByDescending(x => x.Received == null && x.Receipt == null)
                    .ThenByDescending(x => x.Received ?? x.Receipt);
                break;
            case nameof(PurchaseBasicModel.Received):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Received ?? x.Receipt) : query.OrderBy(x => x.Received ?? x.Receipt);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;

        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<PurchaseBasicModel>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<PurchaseBasicModel>(count, list);
    }

    internal override async Task BeforeSaveAsync(Purchase entity)
    {
        var items = await Context.PurchaseItems.Where(x => x.PurchaseId == entity.Id)
            .Select(x => new PurchaseItem() { Id = x.Id, DateTime = x.DateTime })
            .ToListAsync();

        foreach (var item in items) {
            Context.Attach(item);
            item.DateTime = entity.Received ?? entity.Receipt ?? entity.Created;
        }
    }
}