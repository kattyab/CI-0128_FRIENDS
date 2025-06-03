using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IEmployeePayrollRepository
    {
        Task<List<EmployeePayroll>> GetEmployeeDataAsync(Guid companyId);
        Task<Guid> GetPersonPkByEmailAsync(string email);
    }
}
