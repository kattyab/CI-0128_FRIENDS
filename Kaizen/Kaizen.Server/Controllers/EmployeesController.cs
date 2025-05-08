using Kaizen.Server.Repository;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers;

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
