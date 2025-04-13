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

        /// <summary>
        /// Get the life insurance quote based on the date of birth and sex.
        /// </summary>
        /// <param name="dob">The date of birth in the format yyyy-mm-dd.</param>
        /// <param name="sex">The sex of the employee, 'male' or 'female'.</param>
        /// <returns>A life insurance quote based on the provided information.</returns>
        /// <response code="200">Returns the life insurance quote.</response>
        /// <response code="400">Bad Request, missing or invalid parameters.</response>
        /// <response code="403">Forbidden, invalid or missing API token.</response>
        /// <response code="500">Internal Server Error if an unexpected error occurs.</response>
        [HttpGet]
        [Produces("application/json", "application/xml")] // Support for JSON and XML
        [Consumes("application/json", "application/xml")]
        public IActionResult Get([FromQuery] DateTime? dob, [FromQuery] string sex)
        {
            try
            {
                // Token check 403 Forbidden
                if (!Request.Headers.TryGetValue("X-API-TOKEN", out var token) || token != ApiToken)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "403 Forbidden: Invalid or missing API token.");
                }

                // Parameter validation 400 Bad Request
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
