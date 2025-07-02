using System;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeDashboardController(IEmployeeDashboardRepository repository, IAuthService authService) : ControllerBase
{
    private readonly IEmployeeDashboardRepository _repository = repository;
    private readonly IAuthService _authService = authService;

    [HttpGet]
    public IActionResult GetDashboard()
    {
        if (!_authService.IsAuthenticated())
            return Unauthorized(new { message = "Usuario no autenticado." });

        var user = _authService.GetAuthUser();

        var dashboard = _repository.GetDashboardByUserPK(user.UserPK);
        if (dashboard == null || dashboard.UserPK == Guid.Empty)
            return NotFound(new { message = "Empleado no encontrado." });

        return Ok(dashboard);
    }
}
