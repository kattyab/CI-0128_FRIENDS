using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Queries.Benefits;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferedBenefitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfferedBenefitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("available/{email}")]
        public async Task<ActionResult<List<OfferedBenefitDto>>> GetAvailableBenefitsForEmployee(
            string email,
            [FromQuery] bool includeUnavailable = false)
        {
            try
            {
                var query = new OfferedBenefitsQuery(email, includeUnavailable);
                var benefits = await _mediator.Send(query);
                return Ok(benefits);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving offered benefits: {ex.Message}");
            }
        }
    }
}
