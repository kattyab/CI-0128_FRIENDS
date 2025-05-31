using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IEmployeeDeductionRepository
    {
        Dictionary<Guid, EmployeeDto> GetEmployeesByCompany(Guid companyID);
        Dictionary<Guid, List<Guid>> GetChosenBenefitsByCompany(Guid companyID);
    }
}
