using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly Login _handler;
        private readonly string _connectionString;

        public LoginController(Login handler, IConfiguration config)
        {
            _handler = handler;
            _connectionString = config.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException("The connection string 'KaizenDb' is not defined in appsettings.json.");
        }

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
            var userId = (string)user.GetType().GetProperty("UserPK")!.GetValue(user)!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credentials.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId)
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


        [Authorize]
        [HttpGet("payroll-info")]
        public IActionResult GetPayrollInfo()
        {
            var userId = Guid.Parse(User.FindFirst("UserId")!.Value);

            const string sql = @"
                SELECT c.CompanyPK, c.PayrollType
                FROM   Users     u
                JOIN   Companies c ON c.CompanyPK = u.CompanyPK
                WHERE  u.UserPK = @UserId;";

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = userId });

            con.Open();
            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

            if (!reader.Read())
                return NotFound(new { message = "Empresa no encontrada." });

            var companyId = (Guid)reader["CompanyPK"];
            var letter = reader["PayrollType"]?.ToString();   // W | B | M

            return Ok(new
            {
                companyId,
                letter
            });
        }
    }
}
