using MediatR;
using Kaizen.Server.Application.Interfaces.Benefits;

namespace Kaizen.Server.Application.Commands.Benefits
{
    public class SubscribeAPIBenefitCommandHandler : IRequestHandler<SubscribeAPIBenefitCommand>
    {
        private readonly IAPIBenefitSubscriptionRepository _repository;

        public SubscribeAPIBenefitCommandHandler(IAPIBenefitSubscriptionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(SubscribeAPIBenefitCommand request, CancellationToken cancellationToken)
        {
            await _repository.SubscribeAPIBenefitAsync(request);
        }
    }
}
