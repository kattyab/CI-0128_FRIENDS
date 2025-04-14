using KaizenProto.Server.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace KaizenProto.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly EmpleadoHandler _empleadoHandler;

        // Constructor: ASP.NET Core inyecta el servicio EmpleadoHandler
        public EmpleadoController(EmpleadoHandler empleadoHandler)
        {
            _empleadoHandler = empleadoHandler;
        }

        [HttpGet("{cedula}")]
        public IActionResult GetEmpleadoInfo(string cedula)
        {
            var empleado = _empleadoHandler.ObtenerEmpleadoInfo(cedula);
            if (empleado == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(empleado);
        }
    }
}
