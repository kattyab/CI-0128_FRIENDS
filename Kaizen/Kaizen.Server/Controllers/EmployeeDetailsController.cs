using Kaizen.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Models;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly Repository.EmployeeDetailsRepository _employeeHandler;

        public EmployeeDetailsController(Repository.EmployeeDetailsRepository employeeHandler)
        {
            _employeeHandler = employeeHandler;
        }

        [HttpGet("{email}")]
        [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEmployeeInfo(string email)
        {
            var employee = _employeeHandler.ObtainEmployeeData(email);
            if (employee == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(employee);
        }
    }
}