using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api")]
public class RegisterCompanyController : ControllerBase
{
    private readonly RegisterCompanyRepository _registerCompanyRepository;

    public RegisterCompanyController(RegisterCompanyRepository repository)
    {
        _registerCompanyRepository = repository;
    }

    [HttpPost("registerCompany")]
    public async Task<ActionResult<bool>> RegisterCompany([FromBody] RegisterCompanyDto company)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _registerCompanyRepository.CreateCompany(company);
            if (result)
                return Ok(result);

            return BadRequest("No se pudo crear la empresa.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error creando empresa: {ex.Message}");
        }
    }
}
