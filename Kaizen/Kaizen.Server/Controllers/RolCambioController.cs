using Kaizen.Server.Models;
using Kaizen.Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolCambioController : ControllerBase
    {
        private readonly RolCambioHandler _RolCambioHandler;

        public RolCambioController()
        {
            _RolCambioHandler = new RolCambioHandler();
        }

        // Ruta para obtener todos los usuarios y sus roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolCambioModel>>> GetUsuarios()
        {
            try
            {
                var usuarios = _RolCambioHandler.ObtenerUsuarios();  // Llamada al nuevo método
                return Ok(usuarios);  // 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los usuarios. BE");
            }
        }

        // Ruta para cambiar el rol de un usuario por su email
        [HttpPut("cambiar-rol")]
        public async Task<ActionResult<bool>> CambiarRolPorEmail([FromBody] RolCambioModel datos)
        {
            try
            {
                if (datos == null || string.IsNullOrEmpty(datos.Email) || string.IsNullOrEmpty(datos.NuevoRol))
                {
                    return BadRequest("El email y el nuevo rol son requeridos.");
                }

                var resultado = _RolCambioHandler.CambiarRolPorEmail(datos.Email, datos.NuevoRol);
                return new JsonResult(resultado);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al cambiar el rol del usuario.");
            }
        }
    }
}

