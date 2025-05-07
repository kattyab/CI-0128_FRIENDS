using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Repository;
using Kaizen.Server.Models;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonHomepageController : ControllerBase
    {
        private readonly Repository.CommonHomepageRepository _commonHomepageHandler;

        public CommonHomepageController(Repository.CommonHomepageRepository commonHomepageHandler)
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
