using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmployeesRepository _employeesRepository;

    public EmployeesController(IAuthService authService, IEmployeesRepository employeesRepository)
    {
        this._authService = authService;
        this._employeesRepository = employeesRepository;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if (this._authService.IsAuthenticated() == false)
        {
            return this.Unauthorized();
        }

        Guid companyPK = this._authService.GetAuthUserCompanyPK();

        List<EmployeeDto> employees = this._employeesRepository.GetEmployees(companyPK);

        return this.Ok(employees);
    }
}
