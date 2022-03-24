using Microsoft.AspNetCore.Mvc;
using Destuff.Server.Data;

namespace Destuff.Server.Controllers;

public abstract class BaseController : ControllerBase
{
    public ApplicationDbContext Context { get; }

    public BaseController(ApplicationDbContext context)
    {
        Context = context;
    }
}
