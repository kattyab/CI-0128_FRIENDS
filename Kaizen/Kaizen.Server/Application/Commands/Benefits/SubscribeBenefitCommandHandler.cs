using MediatR;
using Kaizen.Server.Application.Interfaces.Benefits;

namespace Kaizen.Server.Application.Commands.Benefits
{
    public class SubscribeBenefitCommandHandler : IRequestHandler<SubscribeBenefitCommand>
    {
        private readonly IBenefitSubscriptionRepository _repository;

        public SubscribeBenefitCommandHandler(IBenefitSubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(SubscribeBenefitCommand request, CancellationToken cancellationToken)
        {
            await _repository.SubscribeAsync(request.Email, request.BenefitId);
        }
    }
}
