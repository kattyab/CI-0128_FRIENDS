namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IBenefitSubscriptionRepository
    {
        Task SubscribeAsync(string email, Guid benefitId);
    }
}
