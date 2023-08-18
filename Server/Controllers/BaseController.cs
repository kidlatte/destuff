using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers;

public abstract class BaseController : ControllerBase
{
    public ApplicationDbContext Context { get; }
    public IMapper Mapper { get; }

    public BaseController(ApplicationDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }
    
    internal void Audit(Entity entity)
    {
        if (string.IsNullOrEmpty(entity.CreatedBy))
        {
            entity.CreatedBy = User.Identity?.Name;
            entity.Created = DateTime.UtcNow;
        }

        entity.Updated = DateTime.UtcNow;
    }
}

public abstract class BaseController<TEntity> : BaseController where TEntity : Entity
{
    internal IQueryable<TEntity> Query => Context.Set<TEntity>();
    internal IIdentityHasher<TEntity> Hasher { get; }

    public BaseController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<TEntity> hasher) : base(context, mapper)
    {
        Hasher = hasher;
    }

    [HttpDelete("{hash}")]
    public async virtual Task<IActionResult> Delete(string hash, [FromServices] IIdentityHasher<TEntity> hasher)
    {
        int id = hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }
}

public abstract class BaseController<TEntity, TModel> : BaseController<TEntity>
    where TEntity : Entity
    where TModel : class, IModel
{
    public BaseController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<TEntity> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpGet("{hash}")]
    public virtual async Task<ActionResult<TModel>> Get(string hash, [FromServices] IIdentityHasher<TEntity> hasher)
    {
        int id = hasher.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

}
