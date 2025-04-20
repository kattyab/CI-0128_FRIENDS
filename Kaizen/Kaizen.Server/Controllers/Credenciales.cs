using Kaizen.Server.Models;
using Kaizen.Server.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Credenciales : ControllerBase
    {
        private readonly Handlers.CredencialesH _employeeHandler;

        // Constructor: ASP.NET Core inyecta el servicio EmpleadoHandler
        public Credenciales(Handlers.CredencialesH employeeHandler)
        {
            _employeeHandler = employeeHandler;
        }

        [HttpGet("{email}")]
        public IActionResult GetEmpleadoInfo(string email)
        {
            var empleado = _employeeHandler.ObtainUserData(email);
            if (empleado == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(empleado);
        }
    }
}
