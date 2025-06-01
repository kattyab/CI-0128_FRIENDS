using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Devuelve información del usuario autenticado, incluyendo su UserPK.
    /// </summary>
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (!_authService.IsAuthenticated())
        {
            return Unauthorized(new { message = "Usuario no autenticado." });
        }

        try
        {
            AuthUserDto authUser = _authService.GetAuthUser();
            return Ok(new
            {
                userPK = authUser.UserPK,
                email = authUser.Email, 
                personPK=authUser.PersonPK,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el usuario.", error = ex.Message });
        }
    }
}
