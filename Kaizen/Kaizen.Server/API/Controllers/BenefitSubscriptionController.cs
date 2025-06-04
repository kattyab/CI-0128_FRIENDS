using MediatR;
using Microsoft.AspNetCore.Mvc;
using Kaizen.Server.Application.Commands.Benefits;

namespace Kaizen.Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenefitSubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitSubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                var command = new SubscribeBenefitCommand
                {
                    Email = request.Email,
                    BenefitId = request.BenefitId
                };

                await _mediator.Send(command);
                return Ok(new { message = "Subscription successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class SubscribeRequest
        {
            public string Email { get; set; } = string.Empty;
            public Guid BenefitId { get; set; }
        }
    }
}
