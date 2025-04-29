using Microsoft.AspNetCore.Mvc;
using LifeInsuranceApi.Models;
using LifeInsuranceApi.Handlers;

namespace LifeInsuranceApi.Controllers
{
    [ApiController]
    [Route("api/LifeInsurance")]
    public class LifeInsuranceRepository : ControllerBase
    {
        private const string _ApiToken = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7";

        private readonly LifeInsuranceHandler _handler;

        public LifeInsuranceRepository()
        {
            _handler = new LifeInsuranceHandler();
        }

        /// <summary>
        /// Gets the life insurance quote based on the date of birth and sex.
        /// </summary>
        /// <param name="dateOfBirthString">The date of birth in the format YYYY-MM-DD.</param>
        /// <param name="sex">The sex of the employee, 'male', 'female', 'hombre', or 'mujer'.</param>
        /// <returns>A life insurance quote based on the provided information.</returns>
        /// <response code="200">Returns the monthly quote.</response>
        /// <response code="400">Bad Request, missing or invalid parameters.</response>
        /// <response code="403">Forbidden, invalid or missing API token.</response>
        /// <response code="500">Internal Server Error if an unexpected error occurs.</response>
        [HttpGet]
        [Produces("application/json", "application/xml")]
        [Consumes("application/json", "application/xml")]
        public IActionResult Get([FromQuery(Name = "date of birth")] string? dateOfBirthString = null, [FromQuery] string? sex = null)
        {
            try
            {
                if (!IsValidToken())
                    return StatusCode(StatusCodes.Status403Forbidden, "403 Forbidden: Invalid or missing API token.");

                if (string.IsNullOrWhiteSpace(dateOfBirthString) && string.IsNullOrWhiteSpace(sex))
                    return BadRequest("400 Bad Request: Parameters empty.");

                var validationResult = ValidateParameters(dateOfBirthString ?? string.Empty, sex ?? string.Empty);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ErrorMessage);

                var result = _handler.Calculate(new LifeInsuranceRequest
                {
                    DateOfBirth = validationResult.DateOfBirth ?? throw new InvalidOperationException("DateOfBirth cannot be null."),
                    Sex = sex ?? throw new InvalidOperationException("Sex cannot be null.")
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"500 Internal Server Error: An error occurred: {ex.Message}");
            }
        }

        private bool IsValidToken() =>
            Request.Headers.TryGetValue("FRIENDS-API-TOKEN", out var token) && token == _ApiToken;

        private ValidationResult ValidateParameters(string dateOfBirthString, string sex)
        {
            var result = new ValidationResult();
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dateOfBirthString))
            {
                errors.Add("Missing date of birth parameter.");
            }
            else if (!DateTime.TryParse(dateOfBirthString, out var parsedDate))
            {
                errors.Add("Invalid date of birth format. Use format YYYY-MM-DD with a valid date.");
            }
            else
            {
                result.DateOfBirth = parsedDate;
            }

            if (string.IsNullOrWhiteSpace(sex))
            {
                errors.Add("Missing 'sex' parameter. Must be 'male', 'female', 'hombre', or 'mujer'.");
            }
            else
            {
                var validSexValues = new[] { "male", "female", "hombre", "mujer" };
                if (!validSexValues.Contains(sex.ToLower()))
                {
                    errors.Add("Invalid 'sex' parameter. Must be 'male', 'female', 'hombre', or 'mujer'.");
                }
            }

            if (errors.Any())
            {
                result.IsValid = false;
                result.ErrorMessage = $"400 Bad Request: {string.Join(" ", errors)}";
            }

            return result;
        }
    }
}
