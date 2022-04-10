using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;

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
    
    public BaseController(ApplicationDbContext context, IMapper mapper): base(context, mapper)
    {
    }
}
