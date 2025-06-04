using Kaizen.Server.Application.Commands.Employees;
using Kaizen.Server.Application.Dtos.Employees;
using Kaizen.Server.Application.Queries.Employees;
using Kaizen.Server.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-id/{empId:guid}")]
        [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployeeById(Guid empId)
        {
            try
            {
                var employee = await _mediator.Send(new GetEmployeeByIdQuery(empId));

                if (employee == null)
                {
                    return NotFound(new { message = "Empleado no encontrado." });
                }

                return Ok(employee);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the employee." });
            }
        }

        [HttpPut("{empId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmployee(Guid empId, [FromBody] EmployeeDetailsDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(new UpdateEmployeeCommand(empId, employeeDto));

                if (!result)
                {
                    return NotFound(new { message = $"Employee with ID {empId} not found" });
                }

                return Ok(new { message = "Employee updated successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the employee" });
            }
        }
    }
}
