using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollCore
    {
        decimal GetLaborChargeRate();
        bool IsPayrollValid(PayrollSummary payrollSummary);
        PayrollResultSumary CreatePayrollResult(PayrollRequest request, List<PayrollSummary> payrollResults);
    }
}
