using AutoMapper;
using Destuff.Server.Data;
using Destuff.Server.Data.Entities;
using Destuff.Server.Services;
using Destuff.Shared;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.Maintenances)]
[ApiController, Authorize]
public class MaintenancesController : BaseController<Maintenance, MaintenanceModel, MaintenanceRequest>
{
    public MaintenancesController(ApplicationDbContext context, IMapper mapper, IIdentityHasher<Maintenance> hasher) : base(context, mapper, hasher)
    {
    }
}
