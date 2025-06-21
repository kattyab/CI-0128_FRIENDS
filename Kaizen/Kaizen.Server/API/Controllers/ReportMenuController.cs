using Kaizen.Server.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportMenuController : ControllerBase
{
    private readonly IAuthService _authService;

    public ReportMenuController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("user-identifiers")]
    public ActionResult<AuthenticationIdentifiersDto> GetAuthIdentifiers()
    {
        try
        {
            var companyId = _authService.GetAuthUserCompanyPK();
            var employeeId = _authService.GetAuthUserEmployeePK();

            var result = new AuthenticationIdentifiersDto
            {
                CompanyPK = companyId,
                EmployeePK = employeeId
            };

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User is not authenticated.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("company-id")]
    public ActionResult<Guid> GetCompanyPK()
    {
        try
        {
            Guid companyId = _authService.GetAuthUserCompanyPK();
            return Ok(companyId);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User is not authenticated.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("employee-id")]
    public ActionResult<Guid> GetEmployeePK()
    {
        try
        {
            Guid employeeId = _authService.GetAuthUserEmployeePK();
            return Ok(employeeId);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User is not authenticated.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
