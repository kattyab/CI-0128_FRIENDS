using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PayrollController : ControllerBase
{
    private readonly IPayrollProcessingService _payroll;
    private readonly GeneralPayrollRepository _repo;

    public PayrollController(IPayrollProcessingService payroll,
                             GeneralPayrollRepository repo)
    {
        _payroll = payroll;
        _repo = repo;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] PayrollRequest dto)
    {
        var result = await _payroll.ProcessCompanyPayrollAsync(dto);

        char mode = dto.Type switch
        {
            "weekly" => 'W',
            "biweekly" => 'B',
            "monthly" => 'M',
            _ => 'U'
        };

        string period = dto.Type == "monthly"
            ? dto.Start.ToString("MM-yyyy")
            : $"{dto.Start:dd-MM-yyyy} → {dto.End:dd-MM-yyyy}";

        /* NUEVA llamada: ya no pasamos company ni paidBy */
        await _repo.SetModeAndPeriodAsync(mode, period);

        return Ok(result);
    }

}

public sealed record PayrollRequest(
    string Email,
    Guid CompanyId,
    DateTime Start,
    DateTime End,
    string Type    // "weekly" | "biweekly" | "monthly"
);
