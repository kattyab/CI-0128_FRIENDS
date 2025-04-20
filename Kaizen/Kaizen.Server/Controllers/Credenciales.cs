// Controllers/CredencialesController.cs
using Kaizen.Server.Handlers;
using Kaizen.Server.Models;

using Microsoft.AspNetCore.Mvc;

namespace KaizenProto.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CredencialesController : ControllerBase
    {
        private readonly CredencialesH _handler;

        public CredencialesController(CredencialesH handler)
            => _handler = handler;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Obtiene los datos del usuario (incluye Password en texto plano, solo para ejemplo)
            var user = _handler.ObtainUserData(dto.Email);
            if (user is null)
                return NotFound(new { message = "Usuario no encontrado." });

            // Aquí `user` es un objeto anónimo con .Password
            var storedPwd = (string)user.GetType().GetProperty("Password")!.GetValue(user)!;

            if (storedPwd != dto.Password)
                return Unauthorized(new { message = "Contraseña incorrecta." });

            // TODO: generar JWT o establecer cookie de sesión
            return Ok(new { message = "Sesión iniciada", user = dto.Email });
        }
    }
}
