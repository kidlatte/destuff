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
public class SuppliersController : BaseController<Supplier, SupplierModel, SupplierRequest>
{
    public SuppliersController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Supplier> hasher) : base(context, mapper, hasher)
    {
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
        query = sortField switch {
            "" => query.OrderByDescending(x => x.Created),
            _ => request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField),
        };
        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<SupplierListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<SupplierListItem>(count, list);
    }

    [HttpGet(ApiRoutes.SupplierBySlug + "/{slug}")]
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
}