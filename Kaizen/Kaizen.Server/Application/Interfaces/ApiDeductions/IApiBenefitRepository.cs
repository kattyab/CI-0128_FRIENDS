using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IApiBenefitRepository
{
    Task<List<APIsDto>> GetBenefitsAsync(Guid companyId, PayrollTransactionContext context = null);
    Task<List<EmployeeBenefitParameterDto>> GetParametersForCompanyAsync(Guid companyId, PayrollTransactionContext context = null);
}
