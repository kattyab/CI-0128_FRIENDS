using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollRepository
    {
        Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email, PayrollTransactionContext context = null);
    }
}
