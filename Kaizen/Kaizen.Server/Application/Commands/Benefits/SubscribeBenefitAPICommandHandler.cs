using MediatR;
using Kaizen.Server.Application.Interfaces.Benefits;

namespace Kaizen.Server.Application.Commands.Benefits
{
    public class SubscribeBenefitAPICommandHandler : IRequestHandler<SubscribeBenefitAPICommand>
    {
        private readonly IBenefitAPISubscriptionRepository _repository;

        public SubscribeBenefitAPICommandHandler(IBenefitAPISubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(SubscribeBenefitAPICommand request, CancellationToken cancellationToken)
        {
            await _repository.SubscribeAPIBenefitAsync(request);
        }
    }
}
