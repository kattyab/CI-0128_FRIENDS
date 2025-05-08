using Kaizen.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Models;

namespace Kaizen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDetailsController : ControllerBase
    {
        private readonly CompanyDetailsRepository _repository;

        public CompanyDetailsController(CompanyDetailsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(CompanyDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var company = await _repository.GetCompanyDetailsByUserEmail(email);
            if (company == null)
            {
                return NotFound(new { message = "Company not found for the given email." });
            }

            return Ok(company);
        }
    }

}
