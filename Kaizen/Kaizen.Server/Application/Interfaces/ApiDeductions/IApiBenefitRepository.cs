using Kaizen.Server.Application.Dtos.ApiDeductions;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IApiBenefitRepository
{
    Task<List<APIsDto>> GetBenefitsAsync(Guid companyId);
    Task<List<EmployeeBenefitParameterDto>> GetParametersForCompanyAsync(Guid companyId);
}
