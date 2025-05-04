using Kaizen.Server.Models;
using Kaizen.Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.Controllers
{
    // Define the route for this controller: api/RolChange
    [Route("api/[controller]")]
    [ApiController]
    public class RolChangeController : ControllerBase
    {
        private readonly RolChangeHandler _RolChangeHandler;

        // Constructor initializes the handler responsible for role logic
        public RolChangeController()
        {
            _RolChangeHandler = new RolChangeHandler();
        }

        // GET: api/RolChange
        // Retrieves all users and their current roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolChangeModel>>> GetUsuarios()
        {
            try
            {
                var usuarios = _RolChangeHandler.ObtenerUsuarios(); // Call to repository method to get users
                return Ok(usuarios); // Return the user list with HTTP 200
            }
            catch (Exception)
            {
                // Return HTTP 500 in case of an error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving users from backend.");
            }
        }

        // PUT: api/RolChange/cambiar-rol
        // Updates the role of a specific user identified by their email
        [HttpPut("cambiar-rol")]
        public async Task<ActionResult<bool>> CambiarRolPorEmail([FromBody] RolChangeModel datos)
        {
            try
            {
                // Validate input
                if (datos == null || string.IsNullOrEmpty(datos.Email) || string.IsNullOrEmpty(datos.NuevoRol))
                {
                    return BadRequest("Email and new role are required.");
                }

                // Call to repository to change the user's role
                var resultado = _RolChangeHandler.CambiarRolPorEmail(datos.Email, datos.NuevoRol);

                // Return the result in JSON format (true/false)
                return new JsonResult(resultado);
            }
            catch (Exception)
            {
                // Return HTTP 500 in case of an error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user role.");
            }
        }
    }
}
