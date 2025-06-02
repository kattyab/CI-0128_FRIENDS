using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Commands.Benefits;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APIBenefitSubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public APIBenefitSubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> SubscribeAPIBenefit([FromBody] SubscribeAPIBenefitDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new SubscribeAPIBenefitCommand
            {
                AssocName = request.AssocName,
                Dependents = request.Dependents,
                Email = request.Email,
                Id = request.Id
            };

            await _mediator.Send(command);
            return Ok();
        }
    }
}
