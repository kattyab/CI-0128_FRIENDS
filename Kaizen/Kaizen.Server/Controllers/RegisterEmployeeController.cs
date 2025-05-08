using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Repository;
using Kaizen.Server.Models;

namespace Kaizen.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterEmployeeController : ControllerBase
    {
        private readonly RegisterEmployeeRepository _registerEmployeeRepository;

        public RegisterEmployeeController(RegisterEmployeeRepository repository)
        {
            _registerEmployeeRepository = repository;
        }

        [HttpPost("registerEmployee")]
        public async Task<ActionResult<bool>> RegisterEmployee(RegisterEmployeeDto employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _registerEmployeeRepository.CreateEmployee(employee);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando empleado");
            }
        }
    }
}