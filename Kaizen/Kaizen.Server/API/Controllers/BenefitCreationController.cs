using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitCreationController : ControllerBase
    {
        private readonly BenefitCreationRepository _BenefitCreationRepository;

        public BenefitCreationController(BenefitCreationRepository repository)
        {
            _BenefitCreationRepository = repository;
        }

        [HttpPost("benefitCreation")]
        public async Task<ActionResult<bool>> RegisterEmployee(BenefitCreationDto benefit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _BenefitCreationRepository.CreateBenefit(benefit);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creando beneficio");
            }
        }
    }
}
