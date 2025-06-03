using Kaizen.Server.Application.Commands.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IBenefitAPISubscriptionRepository
    {
        Task SubscribeAPIBenefitAsync(SubscribeBenefitAPICommand command);
    }
}