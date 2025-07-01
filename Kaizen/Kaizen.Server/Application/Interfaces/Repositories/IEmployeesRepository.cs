using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Dtos.Companies;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Application.Interfaces.Repositories;

public interface IEmployeesRepository
{
    List<EmployeeDto> GetEmployees(Guid companyPK);
}
