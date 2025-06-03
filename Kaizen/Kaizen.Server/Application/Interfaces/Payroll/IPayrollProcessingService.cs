using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IPayrollProcessingService
    {
        Task<PayrollResultSumary> ProcessCompanyPayrollAsync(PayrollRequest payrollInformation);
        Task<List<PayrollSummary>> CalculateCompanyPayrollAsync(PayrollRequest payrollInformation);
    }
}
