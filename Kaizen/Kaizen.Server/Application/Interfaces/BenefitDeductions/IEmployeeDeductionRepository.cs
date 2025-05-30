using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IEmployeeDeductionRepository
    {
        Dictionary<Guid, Employee> GetEmployeesByCompany(Guid companyID);
        Dictionary<Guid, List<Guid>> GetChosenBenefitsByCompany(Guid companyID);
    }
}
