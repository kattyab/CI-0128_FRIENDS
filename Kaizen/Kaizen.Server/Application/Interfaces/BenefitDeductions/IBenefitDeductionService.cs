using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionService
    {
        Task<List<BenefitDeductionResult>> GetBenefitDeductionsForEmployeeAsync(Guid employeeID, PayrollTransactionContext context = null);
        Task<List<BenefitDeductionResult>> GetBenefitDeductionsForEmployeeAsync(Guid employeeID, decimal proporcionalSalary, PayrollTransactionContext context = null);
    }
}
