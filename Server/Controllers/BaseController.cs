using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Destuff.Server.Controllers;

public abstract class BaseController : ControllerBase
{
    public ApplicationDbContext Context { get; }
    public IMapper Mapper { get; }

    protected string? CurrentUserName => User.FindFirstValue(ClaimTypes.Name);

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
        var id = hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        await BeforeDeleteAsync(entity);
        Context.Remove(entity);
        await Context.SaveChangesAsync();
        await AfterDeleteAsync(entity);

        return NoContent();
    }

    internal virtual Task BeforeDeleteAsync(TEntity entity) => Task.CompletedTask;

    internal virtual Task AfterDeleteAsync(TEntity entity) => Task.CompletedTask;
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
        var id = hasher.Decode(hash);
        var query = Query.Where(x => x.Id == id);

        var model = await query
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (model == null)
            return NotFound();

        return model;
    }

}

public abstract class BaseController<TEntity, TModel, TRequest> : BaseController<TEntity, TModel>
    where TEntity : Entity
    where TModel : class, IModel
    where TRequest : class, IRequest
{
    public BaseController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<TEntity> hasher) : base(context, mapper, hasher)
    {
    }

    [HttpPost]
    public virtual async Task<ActionResult<TModel>> Create([FromBody] TRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(request);

        var entity = Mapper.Map<TEntity>(request);
        Context.Add(entity);

        if (entity is ISluggable sluggable)
            sluggable.Slug = sluggable.ToSlug();

        await BeforeCreateAsync(entity, request);
        Audit(entity);
        await Context.SaveChangesAsync();
        await AfterCreateAsync(entity);

        return Mapper.Map<TModel>(entity);
    }

    [HttpPut("{hash}")]
    public virtual async Task<ActionResult<TModel>> Update(string hash, [FromBody] TRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(request);

        var id = Hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Mapper.Map(request, entity);

        if (entity is ISluggable sluggable)
            sluggable.Slug = sluggable.ToSlug();

        await BeforeUpdateAsync(entity, request);
        Audit(entity);
        await Context.SaveChangesAsync();
        await AfterUpdateAsync(entity);

        return Mapper.Map<TModel>(entity);
    }

    internal virtual Task BeforeSaveAsync(TEntity entity, TRequest request) => BeforeSaveAsync(entity);
    internal virtual Task BeforeSaveAsync(TEntity entity) => Task.CompletedTask;
    internal virtual Task AfterSaveAsync(TEntity entity, TRequest request) => AfterSaveAsync(entity);
    internal virtual Task AfterSaveAsync(TEntity entity) => Task.CompletedTask;

    internal virtual Task BeforeCreateAsync(TEntity entity, TRequest request) => BeforeSaveAsync(entity, request);
    internal virtual Task AfterCreateAsync(TEntity entity) => AfterSaveAsync(entity);

    internal virtual Task BeforeUpdateAsync(TEntity entity, TRequest request) => BeforeSaveAsync(entity, request);
    internal virtual Task AfterUpdateAsync(TEntity entity) => AfterSaveAsync(entity);
}
