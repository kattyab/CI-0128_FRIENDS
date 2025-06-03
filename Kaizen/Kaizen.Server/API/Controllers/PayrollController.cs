using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PayrollController : ControllerBase
{
    private readonly IPayrollProcessingService _payrollService;
    private readonly GeneralPayrollRepository _repo;

    public PayrollController(
        IPayrollProcessingService payrollService,
        GeneralPayrollRepository repo)
    {
        _payrollService = payrollService;
        _repo = repo;
    }


    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] PayrollRequest dto)
    {

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


        bool exists = await _repo.ExistsPeriodAsync(dto.CompanyId, mode, period);
        if (exists)
            return BadRequest("Ya se ejecutó la planilla para ese período.");


        var result = await _payrollService.ProcessCompanyPayrollAsync(dto);


        await _repo.SetExtraFieldsAsync(mode, period, dto.Email);

        return Ok(result);
    }


    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] Guid companyId)
    {
        if (companyId == Guid.Empty)
            return BadRequest("Debe proporcionar companyId en la query.");

        var historyRows = await _repo.GetHistoryByCompanyAsync(companyId);
        return Ok(historyRows);
    }
}

public sealed record PayrollRequest(
    string Email,    
    Guid CompanyId, 
    DateTime Start,     
    DateTime End,      
    string Type       
);
