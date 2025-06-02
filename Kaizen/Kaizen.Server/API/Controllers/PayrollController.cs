// API/Controllers/PayrollController.cs
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class PayrollController : ControllerBase
    {
        [HttpPost("process")]
        public IActionResult Process([FromBody] PayrollRequest dto)
        {
            // ── Mock calculation ───────────────────────────────
            var rng = new Random();
            var gross = rng.Next(800_000, 1_400_000);
            var deductions = (int)(gross * 0.15);
            var net = gross - deductions;

            string f(DateTime d) => $"{d:dd-MM-yyyy}";
            var period = $"{f(dto.Start)} → {f(dto.End)}";

            var result = new
            {
                dto.CompanyId,
                manager = dto.Email,    
                dto.Type,
                period,
                gross,
                deductions,
                net
            };

            return Ok(result);
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
