using Destuff.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Destuff.Server.Controllers;

[Route(ApiRoutes.MaintenanceLogs)]
[ApiController, Authorize]
public class MaintenanceLogsController : ControllerBase
{
}
