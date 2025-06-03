using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IBenefitCompanyOfferedListRepository
    {
        Task<List<BenefitCompanyOfferedListDto>> GetAvailableBenefitsForEmployee(string email);
    }
}