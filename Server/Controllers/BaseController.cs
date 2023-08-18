using AutoMapper;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
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

public abstract class BaseController<T> : BaseController where T : Entity
{
    internal IQueryable<T> Query => Context.Set<T>();
    internal IIdentityHasher<T> Hasher { get; }

    public BaseController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<T> hasher) : base(context, mapper)
    {
        Hasher = hasher;
    }

    [HttpDelete("{hash}")]
    public async virtual Task<IActionResult> Delete(string hash)
    {
        int id = Hasher.Decode(hash);
        var entity = await Query.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (entity == null)
            return NotFound();

        Context.Remove(entity);
        await Context.SaveChangesAsync();

        return NoContent();
    }
}
