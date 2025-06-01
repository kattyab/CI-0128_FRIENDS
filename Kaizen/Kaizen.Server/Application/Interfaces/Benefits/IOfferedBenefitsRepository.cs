using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IOfferedBenefitsRepository
    {
        Task<List<OfferedBenefitDto>> GetAvailableBenefitsForEmployee(string email);
    }
}