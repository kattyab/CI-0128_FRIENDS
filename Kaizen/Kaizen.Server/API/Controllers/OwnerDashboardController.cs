using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/owner-dashboard")]
    public class OwnerDashboardController : ControllerBase
    {
        private readonly OwnerDashboardRepository _dashboardRepository;

        public OwnerDashboardController(OwnerDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("contract-counts-last-3-months")]
        public async Task<IActionResult> GetContractCountsLast3Months([FromQuery] Guid companyPk)
        {
            var results = await _dashboardRepository.GetContractCountsLast3MonthsAsync(companyPk);
            return Ok(results);
        }
    }
}
