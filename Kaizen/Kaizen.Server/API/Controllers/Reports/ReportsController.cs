using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPayrollReportsService _payrollReportsService;
        private readonly IAuthService _authService;
        private readonly IReportService _reportService;

        public ReportsController(IPayrollReportsService payrollReportsService,
            IAuthService authService,
            IReportService reportService)
        {
            _payrollReportsService = payrollReportsService;
            _authService = authService;
            _reportService = reportService;
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

        [HttpGet("historicrange/data")]
        public ActionResult<HistoricRangeInitialDto> GetHistoricRangeInitialData()
        {
            try
            {
                if (_authService.IsAuthenticated() == false)
                {
                    return Unauthorized();
                }

                Guid companyPK = _authService.GetAuthUserCompanyPK();
                HistoricRangeInitialDto historicRangeInitialData = _reportService.GetHistoricRangeInitialDataAsync(companyPK);
                return Ok(historicRangeInitialData);
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

        [HttpGet("historicrange/search")]
        public async Task<ActionResult<HistoricRangeInitialDto>> GetHistoricRangeSearch([FromQuery] HistoricRangeSearch historicRangeSearch)
        {
            try
            {
                if (_authService.IsAuthenticated() == false)
                {
                    return Unauthorized();
                }

                Guid companyPK = _authService.GetAuthUserCompanyPK();
                List<HistoricRangePayroll> historicRangePayrolls = await _reportService.GetHistoricRangePayroll(companyPK, historicRangeSearch);
                return Ok(historicRangePayrolls);
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

        [HttpPost("historicrange/email")]
        public async Task<ActionResult> SendHistoricRangeEmail([FromBody] HistoricRangeSearch historicRangeSearch)
        {
            try
            {
                if (_authService.IsAuthenticated() == false)
                {
                    return Unauthorized();
                }

                Guid companyPK = _authService.GetAuthUserCompanyPK();
                await _reportService.SendHistoricRangeEmail(companyPK, historicRangeSearch);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok();
        }

        [HttpPost("companieshistoric/email")]
        public async Task<ActionResult> SendCompaniesHistoricEmail([FromBody] CsvPayloadDto payload)
        {
            try
            {
                if (!this._authService.IsAuthenticated())
                {
                    return this.Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(payload.Csv))
                {
                    return this.BadRequest("El contenido CSV no puede estar vacío.");
                }

                await this._reportService.SendCompaniesHistoricEmail(payload.Csv);
                return this.Ok();
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}