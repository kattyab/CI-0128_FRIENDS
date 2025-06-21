using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Kaizen.Server.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPayrollReportsService _payrollReportsService;

        public ReportsController(IPayrollReportsService payrollReportsService)
        {
            _payrollReportsService = payrollReportsService;
        }

        [HttpGet("company/{companyId:guid}")]
        public async Task<ActionResult<IEnumerable<OwnerPayrollReport>>> GetOwnersPayrollReportByCompany(Guid companyId)
        {
            try
            {
                var summaries = await _payrollReportsService.ExecuteAsync(companyId);

                var reportsWithCharges = summaries
                    .Select(report => _payrollReportsService.CalculateLaborCharges(report))
                    .ToList();

                var orderedReports = OrderReportsByPeriodDescending(reportsWithCharges);

                return Ok(orderedReports);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private List<OwnerPayrollReport> OrderReportsByPeriodDescending(List<OwnerPayrollReport> reports)
        {
            return reports.OrderByDescending(report =>
                DateTime.ParseExact(report.Period, "MM-yyyy", CultureInfo.InvariantCulture)
            ).ToList();
        }

    }
}
