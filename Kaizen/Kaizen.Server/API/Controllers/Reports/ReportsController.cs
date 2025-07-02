using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Services;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IReportService _reportService;

        public ReportsController(IAuthService authService,
            IReportService reportService)
        {
            this._authService = authService;
            this._reportService = reportService;
        }

        [HttpGet("historicrange/data")]
        public ActionResult<HistoricRangeInitialDto> GetHistoricRangeInitialData()
        {
            try
            {
                if (this._authService.IsAuthenticated() == false)
                {
                    return this.Unauthorized();
                }

                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                HistoricRangeInitialDto historicRangeInitialData = this._reportService.GetHistoricRangeInitialDataAsync(companyPK);
                return this.Ok(historicRangeInitialData);
            }
            catch (ArgumentException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("historicrange/search")]
        public async Task<ActionResult<HistoricRangeInitialDto>> GetHistoricRangeSearch([FromQuery] HistoricRangeSearch historicRangeSearch)
        {
            try
            {
                if (this._authService.IsAuthenticated() == false)
                {
                    return this.Unauthorized();
                }

                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                List<HistoricRangePayroll> historicRangePayrolls = await this._reportService.GetHistoricRangePayroll(companyPK, historicRangeSearch);
                return this.Ok(historicRangePayrolls);
            }
            catch (ArgumentException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("historicrange/email")]
        public async Task<ActionResult> SendHistoricRangeEmail([FromBody] HistoricRangeSearch historicRangeSearch)
        {
            try
            {
                if (this._authService.IsAuthenticated() == false)
                {
                    return this.Unauthorized();
                }

                Guid companyPK = this._authService.GetAuthUserCompanyPK();
                await this._reportService.SendHistoricRangeEmail(companyPK, historicRangeSearch);
            }
            catch (ArgumentException ex)
            {
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return this.Ok();
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
