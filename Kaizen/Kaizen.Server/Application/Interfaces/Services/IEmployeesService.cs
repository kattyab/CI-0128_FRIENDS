using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Application.Interfaces.Services;

public interface IEmployeesService
{
    EmployeeDto GetEmployee(Guid companyPK, Guid EmpID);
    void DeleteEmployee(Guid companyPK, Guid EmpID, Guid userId);
}
