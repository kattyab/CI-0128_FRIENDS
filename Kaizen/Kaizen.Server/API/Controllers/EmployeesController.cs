using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services;
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
    private readonly IEmployeesService _employeesService;

    public EmployeesController(
        IAuthService authService,
        IEmployeesRepository employeesRepository,
        IEmployeesService employeesService)
    {
        this._authService = authService;
        this._employeesRepository = employeesRepository;
        this._employeesService = employeesService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if (this._authService.IsAuthenticated() == false)
        {
            return this.Unauthorized();
        }

        Guid companyPK = this._authService.GetAuthUserCompanyPK();

        List<EmployeeDto> employees = this._employeesRepository.GetEmployees(companyPK, false);

        return this.Ok(employees);
    }

    [HttpDelete("{empID}")]
    public IActionResult Delete(Guid empID)
    {
        if (this._authService.IsAuthenticated() == false)
        {
            return this.Unauthorized();
        }

        try
        {
            AuthUserDto authUser = this._authService.GetAuthUser();
            Guid companyPK = this._authService.GetAuthUserCompanyPK();

            this._employeesService.DeleteEmployee(companyPK, empID, authUser.UserPK);

            return this.Ok();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
    }
}
