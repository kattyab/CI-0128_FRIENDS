using Kaizen.Server.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeHandler _employeeHandler;

        // Constructor: ASP.NET Core inyecta el servicio EmpleadoHandler
        public EmployeeController(EmployeeHandler employeeHandler)
        {
            _employeeHandler = employeeHandler;
        }

        [HttpGet("{email}")]
        public IActionResult GetEmpleadoInfo(string email)
        {
            var empleado = _employeeHandler.ObtainEmployeeData(email);
            if (empleado == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(empleado);
        }
    }
}
