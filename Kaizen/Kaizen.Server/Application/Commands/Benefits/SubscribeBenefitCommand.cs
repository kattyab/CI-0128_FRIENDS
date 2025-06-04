using MediatR;

namespace Kaizen.Server.Application.Commands.Benefits
{
    public class SubscribeBenefitCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public Guid BenefitId { get; set; }
    }
}
