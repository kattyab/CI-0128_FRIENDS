using Kaizen.Server.Repository;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController(Login handler) : ControllerBase
    {
        private readonly Login _handler = handler;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto credentials)
        {
            var user = _handler.ObtainUserData(credentials.Email);
            if (user is null)
                return NotFound(new { message = "Usuario no encontrado." });

            var storedPwd = (string)user.GetType().GetProperty("Password")!.GetValue(user)!;

            var hasher = new PasswordHasher<string>();
            // This is how you hash passwords:
            // string sampleHash = hasher.HashPassword(null, "Diosishere");
            var result = hasher.VerifyHashedPassword(
                            credentials.Email,
                            storedPwd,
                            credentials.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Contraseña incorrecta." });
            // TODO: Here you can set a session cookie or generate a JWT token
            return Ok(new { message = "Sesión iniciada", user = credentials.Email });
        }
    }
}
