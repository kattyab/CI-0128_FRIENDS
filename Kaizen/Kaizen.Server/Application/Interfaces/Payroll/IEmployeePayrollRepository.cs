using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IEmployeePayrollRepository
    {
        Task<List<EmployeePayroll>> GetEmployeeDataAsync(PayrollRequest payrollInformation);
        Task<Guid> GetPersonPkByEmailAsync(string email);
    }
}
