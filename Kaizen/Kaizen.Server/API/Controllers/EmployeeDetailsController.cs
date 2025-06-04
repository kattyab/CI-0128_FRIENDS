using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly EmployeeDetailsRepository _employeeHandler;

        public EmployeeDetailsController(EmployeeDetailsRepository employeeHandler)
        {
            _employeeHandler = employeeHandler;
        }

        [HttpGet("by-id/{empId:guid}")]
        [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(Guid empId)
        {
            var employee = await _employeeHandler.ObtainEmployeeDataById(empId);
            if (employee == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(employee);
        }

        [HttpPut("{empId:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid empId, [FromBody] EmployeeDetailsDto employeeDto)
        {
            try
            {
                var result = await _employeeHandler.UpdateEmployeeById(empId, employeeDto);

                if (!result)
                {
                    return NotFound($"Employee with ID {empId} not found");
                }

                return Ok("Employee updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the employee");
            }
        }
    }
}
