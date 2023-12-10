using AutoMapper;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;

namespace Destuff.Server.Models;

public class ControllerParameters
{
    public ApplicationDbContext Context { get; }
    public IMapper Mapper { get; }
    public IDateTimeProvider DateTimeProvider { get; }

    public ControllerParameters(ApplicationDbContext context, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        Context = context;
        Mapper = mapper;
        DateTimeProvider = dateTimeProvider;
    }
}

public class ControllerParameters<TEntity> : ControllerParameters
    where TEntity : Entity
{
    public IIdentityHasher<TEntity> Hasher;

    public ControllerParameters(
        ApplicationDbContext context, 
        IMapper mapper, 
        IDateTimeProvider dateTimeProvider, 
        IIdentityHasher<TEntity> hasher) : base(context, mapper, dateTimeProvider)
    {
        Hasher = hasher;
    }
}
