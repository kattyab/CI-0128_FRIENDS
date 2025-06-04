using MediatR;

namespace Kaizen.Server.Application.Commands.Benefits
{
    public class SubscribeBenefitAPICommand : IRequest
    {
        public string? AssocName { get; set; }
        public string? Dependents { get; set; }
        public string Email { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
