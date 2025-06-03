using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovedHoursController(ApprovedHoursRepository approvedHoursRepository) : ControllerBase
{
    private readonly ApprovedHoursRepository _approvedHoursRepository = approvedHoursRepository;

    [HttpPost("")]
    public IActionResult Create([FromBody] ApprovedHoursDto dto)
    {
        if (dto == null)
        {
            return this.BadRequest("Invalid data.");
        }

        try
        {
            this._approvedHoursRepository.InsertApprovedHour(dto);
            return this.Ok(new { message = "Approved hour record inserted successfully." });
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("approved-hours/{empId}")]
    public IActionResult GetApprovedHoursByEmpId(Guid empId)
    {
        var hours = _approvedHoursRepository.GetApprovedHoursByEmpId(empId);
        return Ok(hours);
    }

    [HttpPatch("{approvalID}")]
    public async Task<IActionResult> UpdateStatusAndSent(Guid approvalID, [FromBody] ApprovedHoursDto dto)
    {
        var result = await _approvedHoursRepository.UpdateStatusAndSentAsync(approvalID, dto.Status, dto.IsSentForApproval);

        if (!result)
            return NotFound(new { message = "Registro no encontrado." });

        return Ok(new { message = "Estado actualizado correctamente." });
    }

}
