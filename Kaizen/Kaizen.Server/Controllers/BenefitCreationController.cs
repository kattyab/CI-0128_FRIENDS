using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Repository;
using Kaizen.Server.Models;

namespace Kaizen.Server.Controllers
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
