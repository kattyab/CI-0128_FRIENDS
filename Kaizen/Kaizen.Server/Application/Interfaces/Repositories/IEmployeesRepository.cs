using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Repositories;

public interface IEmployeesRepository
{
    List<EmployeeDto> GetEmployees(Guid companyPK, bool showDeleted);
    EmployeeDto? GetEmployee(Guid companyPK, Guid empID);
    void DeleteEmployee(Guid companyPK, Guid EmpID, Guid userId);
}
