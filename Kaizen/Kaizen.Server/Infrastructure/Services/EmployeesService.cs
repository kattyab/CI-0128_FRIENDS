using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services;

namespace Kaizen.Server.Infrastructure.Services;

public class EmployeesService : IEmployeesService
{
    private readonly IEmployeesRepository _employeesRepository;

    public EmployeesService(IEmployeesRepository employeesRepository)
    {
        this._employeesRepository = employeesRepository;
    }

    public EmployeeDto GetEmployee(Guid companyPK, Guid EmpID)
    {
        EmployeeDto? employee = this._employeesRepository.GetEmployee(companyPK, EmpID);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {EmpID} not found in company {companyPK}.");
        }
        return employee;
    }

    public void DeleteEmployee(Guid companyPK, Guid EmpID, Guid userId)
    {
        if (this.GetEmployee(companyPK, EmpID) == null)
        {
            throw new KeyNotFoundException($"Employee with ID {EmpID} not found in company {companyPK}.");
        }

        this._employeesRepository.DeleteEmployee(companyPK, EmpID, userId);
    }
}
