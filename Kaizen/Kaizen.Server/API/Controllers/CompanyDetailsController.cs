using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers
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
