using Kaizen.Server.Application.Dtos.ApiDeductions;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IBenefitRepository
{
    Task<List<BenefitDto>> GetBenefitsAsync(Guid companyId);
    Task<List<EmployeeBenefitParameterDto>> GetParametersForEmployeeAsync(Guid employeeId);
}
