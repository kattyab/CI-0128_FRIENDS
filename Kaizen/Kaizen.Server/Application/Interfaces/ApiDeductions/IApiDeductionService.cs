using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions
{
    public interface IApiDeductionService
    {
        Task<Dictionary<string, decimal>> GetDeductionsForEmployeeAsync(Guid employeeId, PayrollTransactionContext context = null);
    }
}
