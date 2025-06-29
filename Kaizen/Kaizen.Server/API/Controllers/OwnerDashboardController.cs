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

        [HttpGet("last-3-payrolls")]
        public async Task<IActionResult> GetLast3Payrolls([FromQuery] Guid companyPk)
        {
            var results = await _dashboardRepository.GetLast3PayrollsAsync(companyPk);
            return Ok(results);
        }

        [HttpGet("payroll-cost-breakdown")]
        public async Task<IActionResult> GetPayrollCostBreakdown([FromQuery] Guid companyPk)
        {
            var result = await _dashboardRepository.GetPayrollCostBreakdownAsync(companyPk);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
