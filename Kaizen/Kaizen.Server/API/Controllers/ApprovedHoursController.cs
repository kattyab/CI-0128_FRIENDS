using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovedHoursController(ApprovedHoursRepository approvedHoursRepository) : ControllerBase
{
    private readonly ApprovedHoursRepository _approvedHoursRepository = approvedHoursRepository;

    // POST: api/ApprovedHours
    // This method handles HTTP POST requests to insert a new approved hour record
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
}
