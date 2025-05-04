using Kaizen.Server.Repository;
using Kaizen.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Kaizen.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController(Login handler) : ControllerBase
    {
        private readonly Login _handler = handler;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto credentials)
        {
            var user = _handler.ObtainUserData(credentials.Email);
            if (user is null)
                return NotFound(new { message = "Usuario o contraseña incorrecta." });

            var storedPwd = (string)user.GetType().GetProperty("PasswordHash")!.GetValue(user)!;

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(
                            credentials.Email,
                            storedPwd,
                            credentials.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Usuario o contraseña incorrecta." });

            var role = (string)user.GetType().GetProperty("Role")!.GetValue(user)!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return Ok(new
            {
                message = "Sesión iniciada",
                user = credentials.Email,
                role
            });
        }
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {
            if (HttpContext.User.Identity?.IsAuthenticated != true)
                return Unauthorized(new { message = "No autenticado" });

            var email = HttpContext.User.Identity?.Name;
            var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { email, role });
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return Ok(new { message = "Sesión cerrada" });
        }
    }
}
