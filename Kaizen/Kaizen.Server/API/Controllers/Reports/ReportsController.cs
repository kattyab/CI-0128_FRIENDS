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

                return Ok(reportsWithCharges);
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

        [HttpGet("employee/{employeeId:guid}")]
        public async Task<ActionResult<IEnumerable<EmployeePayrollReport>>> GetEmployeePayrollReportsByEmployee(Guid employeeId)
        {
            try
            {
                var reports = await _payrollReportsService.ExecuteEmployeeAsync(employeeId);
                
                var reportsWithObligatoryDeductions = reports
                    .Select(report => _payrollReportsService.CalculateObligatoryDeductions(report))
                    .ToList();

                var reportsWithAllDeductions = (await Task.WhenAll(
                    reportsWithObligatoryDeductions.Select(report =>
                        _payrollReportsService.CalculateOptionalDeductionsAsync(report))
                )).ToList();

                return Ok(reportsWithAllDeductions);
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
    }
}
