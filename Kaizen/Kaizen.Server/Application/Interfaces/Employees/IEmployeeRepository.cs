using Kaizen.Server.Application.Dtos.Employees;

namespace Kaizen.Server.Application.Interfaces.Employees
{
    public interface IEmployeeRepository
    {
        Task<EmployeeDetailsDto?> GetByIdAsync(Guid empId);
        Task<bool> UpdateAsync(Guid empId, EmployeeDetailsDto employeeDto);
        Task<bool> ExistsAsync(Guid empId);
    }
}
