using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Queries.Benefits;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitCompanyOfferedListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitCompanyOfferedListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("available/{email}")]
        public async Task<ActionResult<List<BenefitCompanyOfferedListDto>>> GetAvailableBenefitsForEmployee(
            string email,
            [FromQuery] bool includeUnavailable = false)
        {
            try
            {
                var query = new BenefitCompanyOfferedListQuery(email, includeUnavailable);
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
