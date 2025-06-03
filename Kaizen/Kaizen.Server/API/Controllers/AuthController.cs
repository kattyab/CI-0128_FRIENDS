using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, UserInfoRepository userInfoRepo) : ControllerBase
{

    [HttpGet("userinfo")]
    public IActionResult GetUserInfo()
    {
        if (!authService.IsAuthenticated())
            return Unauthorized(new { message = "Usuario no autenticado." });

        var user = authService.GetAuthUser();
        var userInfo = userInfoRepo.GetUserInfo(user.UserPK);

        if (userInfo == null)
            return NotFound(new { message = "No se encontró información del usuario." });

        return Ok(userInfo);
    }

    [HttpGet("")]
    public IActionResult GetCurrentUser()
    {
        if (!authService.IsAuthenticated())
            return Unauthorized(new { message = "Usuario no autenticado." });

        try
        {
            var user = authService.GetAuthUser();
            return Ok(new
            {
                userPK = user.UserPK,
                email = user.Email,
                personPK = user.PersonPK,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el usuario.", error = ex.Message });
        }
    }
}
