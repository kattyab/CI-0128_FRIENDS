using Microsoft.AspNetCore.Mvc;
using LifeInsuranceApi.Models;
using LifeInsuranceApi.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeInsuranceApi.Controllers
{
    /// <summary>
    /// Controller that provides life insurance quotes based on user demographics.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LifeInsuranceController : ControllerBase
    {
        private const string ApiToken = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7";

        /// <summary>
        /// Get the life insurance quote based on the date of birth and sex.
        /// </summary>
        /// <param name="dobString">The date of birth in the format YYYY-MM-DD.</param>
        /// <param name="sex">The sex of the employee, 'male' or 'female'.</param>
        /// <returns>A life insurance quote based on the provided information.</returns>
        /// <response code="200">Returns the life insurance quote.</response>
        /// <response code="400">Bad Request, missing or invalid parameters.</response>
        /// <response code="403">Forbidden, invalid or missing API token.</response>
        /// <response code="500">Internal Server Error if an unexpected error occurs.</response>
        [HttpGet]
        [Produces("application/json", "application/xml")] // Support for JSON and XML
        [Consumes("application/json", "application/xml")]
        public IActionResult Get([FromQuery(Name = "dob")] string dobString = null, [FromQuery] string sex = null)
        {
            try
            {
                // Token check 403 Forbidden
                if (!Request.Headers.TryGetValue("FRIENDS-API-TOKEN", out var token) || token != ApiToken)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "403 Forbidden: Invalid or missing API token.");
                }

                // Parameter check 400 Bad Request
                // Check if both parameters are empty
                if (string.IsNullOrWhiteSpace(dobString) && string.IsNullOrWhiteSpace(sex))
                {
                    return BadRequest("400 Bad Request: Parameters empty.");
                }

                // Parse date of birth with validation
                DateTime? dob = null;
                if (!string.IsNullOrWhiteSpace(dobString))
                {
                    if (!DateTime.TryParse(dobString, out var parsedDate))
                    {
                        return BadRequest("400 Bad Request: Invalid 'dob' format. Use format YYYY-MM-DD with a valid date.");
                    }
                    dob = parsedDate;
                }

                // Individual parameter validation
                var validationErrors = new List<string>();

                if (dob == null)
                    validationErrors.Add("400 Bad Request: Missing or invalid 'dob' (date of birth). Use format YYYY-MM-DD.");

                if (string.IsNullOrWhiteSpace(sex))
                    validationErrors.Add("400 Bad Request: Missing 'sex' parameter. Must be 'male' or 'female'.");
                else if (sex.ToLower() != "male" && sex.ToLower() != "female")
                    validationErrors.Add("400 Bad Request: Invalid 'sex' parameter. Must be 'male' or 'female'.");

                if (validationErrors.Any())
                {
                    return BadRequest(string.Join(" ", validationErrors));
                }

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
