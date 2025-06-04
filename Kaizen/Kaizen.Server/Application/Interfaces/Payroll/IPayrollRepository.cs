using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollRepository
    {
        Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email);
    }
}
