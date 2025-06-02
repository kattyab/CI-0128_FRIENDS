using Kaizen.Server.Application.Commands.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IAPIBenefitSubscriptionRepository
    {
        Task SubscribeAPIBenefitAsync(SubscribeAPIBenefitCommand command);
    }
}