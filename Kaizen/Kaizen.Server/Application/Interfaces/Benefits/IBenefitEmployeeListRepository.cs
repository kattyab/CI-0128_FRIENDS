using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IBenefitEmployeeListRepository
    {
        Task<List<BenefitEmployeeListDto>> GetEmployeeBenefitList(string email);
    }
}
