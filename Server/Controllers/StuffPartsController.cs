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

[Route(ApiRoutes.StuffParts)]
[ApiController, Authorize]
public class StuffPartsController : BaseController
{
    private IIdentityHasher<Stuff> StuffHasher { get; }

    public StuffPartsController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Stuff> stuffHasher) : base(context, mapper)
    {
        StuffHasher = stuffHasher;
    }

    [HttpGet(ApiRoutes.StuffParts + "/{parentHash}")]
    public async Task<PagedList<StuffPartListItem>> Get(string parentHash, [FromQuery] ListRequest? request)
    {
        var parentId = StuffHasher.Decode(parentHash);

        var query = Context.StuffParts.Where(x => x.ParentId == parentId);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Part!.Name.ToLower().Contains(search) ||
                x.Part!.Url!.ToLower().Contains(search) ||
                x.Part!.Notes!.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        switch (sortField) {
            case "":
                break;
            case nameof(StuffPartListItem.Part):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Part!.Name) : query.OrderBy(x => x.Part!.Name);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        var count = await query.CountAsync();
        var list = await query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffPartListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedList<StuffPartListItem>(count, list);
    }

    [HttpGet(ApiRoutes.StuffParents + "/{partHash}")]
    public Task<List<StuffListItem>> GetParents(string partHash, [FromQuery] ListRequest? request)
    {
        var partId = StuffHasher.Decode(partHash);

        var query = Context.StuffParts.Where(x => x.PartId == partId).Select(x => x.Parent!);

        request ??= new ListRequest();
        if (!string.IsNullOrEmpty(request.Search)) {
            var search = request.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search) ||
                x.Url!.ToLower().Contains(search) ||
                x.Notes!.ToLower().Contains(search));
        }

        var sortField = request.SortField ?? "";
        switch (sortField) {
            case "":
                break;
            case nameof(StuffListItem.Name):
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                break;
            default:
                query = request.SortDir == SortDirection.Descending ? query.OrderByDescending(sortField) : query.OrderBy(sortField);
                break;
        }

        return query
            .Skip(request.Skip).Take(request.Take)
            .ProjectTo<StuffListItem>(Mapper.ConfigurationProvider)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<StuffPartModel>> Create([FromBody] StuffPartRequest model)
    {
        if (!ModelState.IsValid || model.ParentId == null || model.PartId == null)
            return BadRequest(model);

        var parentId = StuffHasher.Decode(model.ParentId);
        var partId = StuffHasher.Decode(model.PartId);

        var exists = await Context.StuffParts.AnyAsync(x => x.ParentId == parentId && x.PartId == partId);
        if (exists)
            return BadRequest("Part already exists.");

        var entity = Mapper.Map<StuffPart>(model);
        Context.Add(entity);
        await Context.SaveChangesAsync();

        return await Context.StuffParts
            .Where(x => x.ParentId == entity.ParentId && x.PartId == entity.PartId)
            .ProjectTo<StuffPartModel>(Mapper.ConfigurationProvider)
            .FirstAsync();
    }

    [HttpPut("{parentHash}/{partHash}")]
    public async Task<ActionResult<StuffPartModel>> Update(string parentHash, string partHash, [FromBody] StuffPartRequest request)
    {
        if (!ModelState.IsValid || request.ParentId == null || request.PartId == null)
            return BadRequest(request);

        var parentId = StuffHasher.Decode(parentHash);
        var partId = StuffHasher.Decode(partHash);

        var entity = await Context.StuffParts.FirstOrDefaultAsync(x => x.ParentId == parentId && x.PartId == partId);
        if (entity == null)
            return BadRequest("Location does not exist.");

        if (partHash == request.PartId) {
            Mapper.Map(request, entity);
        }
        else {
            // part changed
            var oldEntity = entity;
            Context.Remove(oldEntity);

            entity = Mapper.Map<StuffPart>(request);
            Context.Add(entity);
        }
        await Context.SaveChangesAsync();

        return await Context.StuffParts
            .Where(x => x.ParentId == entity.ParentId && x.PartId == entity.PartId)
            .ProjectTo<StuffPartModel>(Mapper.ConfigurationProvider)
            .FirstAsync();
    }


    [HttpDelete("{parentHash}/{partHash}")]
    public async Task<IActionResult> Delete(string parentHash, string partHash)
    {
        var parentId = StuffHasher.Decode(parentHash);
        var partId = StuffHasher.Decode(partHash);
        var entity = await Context.StuffParts.Where(x => x.ParentId == parentId && x.PartId == partId).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }

}
