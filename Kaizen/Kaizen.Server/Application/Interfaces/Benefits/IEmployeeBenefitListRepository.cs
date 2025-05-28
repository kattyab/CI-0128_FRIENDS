using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Interfaces.Benefits
{
    public interface IEmployeeBenefitListRepository
    {
        Task<List<EmployeeBenefitListDto>> GetEmployeeBenefitList(string email);
    }
}
