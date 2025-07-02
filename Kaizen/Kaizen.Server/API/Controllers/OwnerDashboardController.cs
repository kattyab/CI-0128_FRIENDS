using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Interfaces.Services.OwnerDashboard;
using System.Threading.Tasks;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/owner-dashboard")]
    public class OwnerDashboardController : ControllerBase
    {
        private readonly IOwnerDashboardService _dashboardService;

        public OwnerDashboardController(IOwnerDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        [HttpGet("contract-counts-last-3-months")]
        public async Task<IActionResult> GetContractCountsLast3Months([FromQuery] Guid companyPk)
        {
            var results = await _dashboardService.GetContractCountsLast3MonthsAsync(companyPk);
            return Ok(results);
        }


        [HttpGet("last-3-payrolls")]
        public async Task<IActionResult> GetLast3Payrolls([FromQuery] Guid companyPk)
        {
            var results = await _dashboardService.GetLast3PayrollsAsync(companyPk);
            return Ok(results);
        }

        [HttpGet("payroll-cost-breakdown")]
        public async Task<IActionResult> GetPayrollCostBreakdown([FromQuery] Guid companyPk)
        {
            var result = await _dashboardService.GetPayrollCostBreakdownAsync(companyPk);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
