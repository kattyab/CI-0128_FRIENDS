using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollSummaryCalculator
    {
        Task<PayrollSummary> CalculatePayrollAsync(EmployeePayroll employee, PayrollRequest request, PayrollTransactionContext context = null);
    }

}
