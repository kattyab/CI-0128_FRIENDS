using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Dtos.Reports;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollRepository
    {
        Task SavePayrollAsync(Guid companyId, List<PayrollSummary> summaries, string email);
        List<HistoricRangePayroll> GetEmployeeHistoricRangePayrolls(Guid employeeId, DateTime start, DateTime end);
    }
}
