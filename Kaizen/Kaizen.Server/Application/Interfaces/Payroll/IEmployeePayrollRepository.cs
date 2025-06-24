using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IEmployeePayrollRepository
    {
        Task<List<EmployeePayroll>> GetEmployeeDataAsync(PayrollRequest payrollInformation, PayrollTransactionContext context = null);
        Task<Guid> GetPersonPkByEmailAsync(string email, PayrollTransactionContext context = null);
    }
}
