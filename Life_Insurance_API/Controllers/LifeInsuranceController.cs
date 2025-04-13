using Microsoft.AspNetCore.Mvc;
using LifeInsuranceApi.Models;
using LifeInsuranceApi.Handlers;

namespace LifeInsuranceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LifeInsuranceController : ControllerBase
    {
        private const string ApiToken = "thePowerOfFRIENDSHIP";

        [HttpGet]
        public IActionResult Get([FromQuery] DateTime? dob, [FromQuery] string sex)
        {
            try
            {
                // Token check
                if (!Request.Headers.TryGetValue("X-API-TOKEN", out var token) || token != ApiToken)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "403 Forbidden: Invalid or missing API token.");
                }

                // Parameter validation
                if (dob == null)
                    return BadRequest("400 Bad Request: Missing or invalid 'dob' (date of birth). Use format yyyy-MM-dd.");
                if (string.IsNullOrWhiteSpace(sex) || (sex.ToLower() != "male" && sex.ToLower() != "female"))
                    return BadRequest("400 Bad Request: Invalid 'sex'. Must be 'male' or 'female'.");

                var request = new LifeInsuranceRequest
                {
                    DateOfBirth = dob.Value,
                    Sex = sex
                };

                var handler = new LifeInsuranceHandler();
                var result = handler.Calculate(request);

                return Ok(result); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"500 Internal Server Error: An error occurred: {ex.Message}"); // 500 Exception
            }
        }
    }
}
