using Kaizen.Server.Application.Dtos.Payroll;
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
    private readonly IPayrollProcessingService _payrollService;
    private readonly GeneralPayrollRepository _repo;

    public PayrollController(
        IPayrollProcessingService payrollService,
        GeneralPayrollRepository repo)
    {
        _payrollService = payrollService;
        _repo = repo;
    }

    /// <summary>
    /// POST api/payroll/process
    /// Procesa la planilla, luego actualiza la última fila en GeneralPayrolls
    /// con PayrollMode, Period e InCharge.
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] PayrollRequest dto)
    {
        // 1) Ejecuta lógica de cálculo de planilla
        var result = await _payrollService.ProcessCompanyPayrollAsync(dto);

        // 2) Determina el modo (W, B o M) según dto.Type
        char mode = dto.Type switch
        {
            "weekly" => 'W',
            "biweekly" => 'B',
            "monthly" => 'M',
            _ => 'U'
        };

        // 3) Formatea el período como MM-yyyy si es mensual, o rango dd-MM-yyyy → dd-MM-yyyy
        string period = dto.Type == "monthly"
            ? dto.Start.ToString("MM-yyyy")
            : $"{dto.Start:dd-MM-yyyy} → {dto.End:dd-MM-yyyy}";

        // 4) dto.Email viene del front-end y lo usamos como InCharge
        await _repo.SetExtraFieldsAsync(mode, period, dto.Email);

        // 5) Retorna el resultado original del cálculo
        return Ok(result);
    }

    /// <summary>
    /// GET api/payroll/history
    /// Devuelve el historial de planillas (últimas primero).
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var historyRows = await _repo.GetHistoryAsync();
        return Ok(historyRows);
    }
}

/// <summary>
/// DTO para petición de procesamiento de planilla.
/// </summary>
public sealed record PayrollRequest(
    string Email,      // Correo del encargado (InCharge)
    Guid CompanyId,  // PK de la compañía (se usa dentro del servicio)
    DateTime Start,      // Fecha de inicio del período
    DateTime End,        // Fecha de fin del período
    string Type        // "weekly" | "biweekly" | "monthly"
);
