using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollOutputService
    {
        void PrintPayrollSummary(PayrollSummary payrollSummary);
        void PrintPayrollResults(List<PayrollSummary> payrollResults);
    }
}
