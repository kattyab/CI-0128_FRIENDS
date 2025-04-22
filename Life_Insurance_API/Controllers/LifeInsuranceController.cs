using Microsoft.AspNetCore.Mvc;
using LifeInsuranceApi.Models;
using LifeInsuranceApi.Handlers;

namespace LifeInsuranceApi.Controllers
{
    /// <summary>
    /// Controller that provides life insurance quotes based on user demographics.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LifeInsuranceController : ControllerBase
    {
        /// <summary>
        /// The API token used for authentication, educational, not real, don't flag.
        /// </summary>
        private const string _ApiToken = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7";

        /// <summary>
        /// The handler responsible for processing life insurance requests.
        /// </summary>
        private readonly LifeInsuranceHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifeInsuranceController"/> class.
        /// </summary>
        public LifeInsuranceController()
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

        /// <summary>
        /// Validates the API token provided in the request headers.
        /// </summary>
        /// <returns><c>true</c> if the token is valid; otherwise, <c>false</c>.</returns>
        private bool IsValidToken() =>
            Request.Headers.TryGetValue("FRIENDS-API-TOKEN", out var token) && token == _ApiToken;

        /// <summary>
        /// Validates the input parameters for the life insurance quote request.
        /// </summary>
        /// <param name="dateOfBirthString">The date of birth as a string.</param>
        /// <param name="sex">The sex of the person requesting insurance.</param>
        /// <returns>A <see cref="ValidationResult"/> containing the validation outcome.</returns>
        private ValidationResult ValidateParameters(string dateOfBirthString, string sex)
        {
            var result = new ValidationResult();
            var errors = new List<string>();

            // Validate date of birth
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

            // Validate sex
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
