using AutoMapper;
using AutoMapper.QueryableExtensions;
using Destuff.Server.Data;
using Destuff.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Destuff.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<DashboardModel> Get()
        {
            var stuffCount = await Context.Stuffs.CountAsync();
            var locationCount = await Context.Locations.CountAsync();
            var inventoriedInMonth = await Context.Events
                .Where(x => x.DateTime >= DateTime.UtcNow.AddMonths(-1))
                .CountAsync();

            var latestStuffs = await Context.Stuffs.OrderByDescending(x => x.Id).Take(10)
                .ProjectTo<StuffBasicModel>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return new DashboardModel { 
                LatestStuffs = latestStuffs, 
                StuffCount = stuffCount,
                InventoriedInMonth = inventoriedInMonth,
                LocationCount = locationCount 
            };
        }
    }
}
