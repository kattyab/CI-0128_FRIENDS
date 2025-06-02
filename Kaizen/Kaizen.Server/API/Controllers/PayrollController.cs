// API/Controllers/PayrollController.cs
using Kaizen.Server.Application.Services.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class PayrollController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPayrollProcessingService _payrollProcessingService;

        public PayrollController(
            IConfiguration configuration,
            IPayrollProcessingService payrollProcessingService)
        {
            _configuration = configuration;
            _payrollProcessingService = payrollProcessingService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] PayrollRequest dto)
        {
            Console.WriteLine(dto.CompanyId);

            var payrollResult = await _payrollProcessingService.ProcessCompanyPayrollAsync(dto);

            return Ok(payrollResult);
        }
    }

    public sealed record PayrollRequest(
        string Email,      
        Guid CompanyId,
        DateTime Start,
        DateTime End,
        string Type       // weekly | biweekly | monthly
    );
}
