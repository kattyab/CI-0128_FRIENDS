using Kaizen.Server.Handlers;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

namespace KaizenProto.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly Login _handler;

        public LoginController(Login handler)
            => _handler = handler;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Obtiene los datos del usuario (incluye Password en texto plano)
            var user = _handler.ObtainUserData(dto.Email);
            if (user is null)
                return NotFound(new { message = "Usuario no encontrado." });

            // Aquí `user` es un objeto anónimo con .Password
            var storedPwd = (string)user.GetType().GetProperty("Password")!.GetValue(user)!;

            var hasher = new PasswordHasher<string>();

            // Así se hashean las passwords: string hashPrueba = hasher.HashPassword(null, "Diosishere");

            var resultado = hasher.VerifyHashedPassword(null, storedPwd, dto.Password);

            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Contraseña incorrecta." });

            // TODO: Aquí se puede establecer una cookie de sesión o un token JWT

            return Ok(new { message = "Sesión iniciada", user = dto.Email });
        }
    }
}
