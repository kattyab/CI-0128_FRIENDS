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
    /// Si el período ya existe, devuelve 400. Si no, procesa y actualiza.
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] PayrollRequest dto)
    {
        // 1) Determinar modo y texto de período
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

        // 2) Verificar existencia previa
        bool exists = await _repo.ExistsPeriodAsync(dto.CompanyId, mode, period);
        if (exists)
            return BadRequest("Ya se ejecutó la planilla para ese período.");

        // 3) Ejecutar lógica de cálculo de planilla
        var result = await _payrollService.ProcessCompanyPayrollAsync(dto);

        // 4) Actualizar la última fila con los datos extra
        await _repo.SetExtraFieldsAsync(mode, period, dto.Email);

        return Ok(result);
    }

    /// <summary>
    /// GET api/payroll/history
    /// Devuelve el historial completo de planillas.
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var historyRows = await _repo.GetHistoryAsync();
        return Ok(historyRows);
    }
}

public sealed record PayrollRequest(
    string Email,     // Correo del encargado
    Guid CompanyId, // CompanyPK
    DateTime Start,     // Fecha de inicio del período
    DateTime End,       // Fecha de fin del período
    string Type       // "weekly" | "biweekly" | "monthly"
);
