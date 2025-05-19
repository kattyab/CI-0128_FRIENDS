using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(EmployeesRepository employeesRepository) : ControllerBase
{
    private readonly EmployeesRepository _employeesRepository = employeesRepository;

    [HttpGet("")]
    public IActionResult Index()
    {
        List<EmployeeDto> employees = this._employeesRepository.GetEmployees();

        return this.Ok(employees);
    }
}
