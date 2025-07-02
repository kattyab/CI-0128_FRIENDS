using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IEmployeeDeductionRepository
    {
        Task<Dictionary<Guid, EmployeeDto>> GetEmployeesByCompanyAsync(Guid companyID, PayrollTransactionContext context = null);
        Task<Dictionary<Guid, List<Guid>>> GetChosenBenefitsByCompanyAsync(Guid companyID, PayrollTransactionContext context = null);
    }
}
