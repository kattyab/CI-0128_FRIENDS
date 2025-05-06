using Kaizen.Server.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDetails : ControllerBase
    {
        private readonly Repository.EmployeeDetails _employeeHandler;

        // Constructor: ASP.NET Core inyecta el servicio EmpleadoHandler
        public EmployeeDetails(Repository.EmployeeDetails employeeHandler)
        {
            _employeeHandler = employeeHandler;
        }

        [HttpGet("{email}")]
        public IActionResult GetEmployeeInfo(string email)
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
