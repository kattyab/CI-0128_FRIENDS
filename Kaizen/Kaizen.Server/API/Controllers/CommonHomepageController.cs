using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonHomepageController : ControllerBase
    {
        private readonly CommonHomepageRepository _commonHomepageHandler;

        public CommonHomepageController(CommonHomepageRepository commonHomepageHandler)
        {
            _commonHomepageHandler = commonHomepageHandler;
        }

        [HttpGet("{email}")]
        [ProducesResponseType(typeof(CommonHomepageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEmployeeInfo(string email)
        {
            Console.Write(email);
            var employee = _commonHomepageHandler.ObtainEmployeeData(email);
            if (employee == null)
            {
                return NotFound(new { message = "Empleado no encontrado." });
            }

            return Ok(employee);
        }
    }
}
