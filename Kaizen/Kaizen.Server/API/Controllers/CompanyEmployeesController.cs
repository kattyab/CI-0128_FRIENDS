using Kaizen.Server.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CompanyEmployeesController : ControllerBase
{
    private readonly CompanyEmployeesRepository _repository;

    public CompanyEmployeesController(CompanyEmployeesRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("by-owner-email/{email}")]
    [ProducesResponseType(typeof(List<CompanyEmployeeSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees(string email)
    {
        var employees = await _repository.GetEmployeesByOwnerEmail(email);
        return Ok(employees);
    }
}
