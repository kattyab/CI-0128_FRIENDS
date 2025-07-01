using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class EmployeesRepository(IConfiguration configuration) : IEmployeesRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

    public List<EmployeeDto> GetEmployees(Guid companyPK)
    {
        const string companyCommandText = @"
            SELECT
                EmpID,
                PersonPK,
                WorksFor,
                JobPosition,
                ContractType
            FROM
                Employees
            WHERE
                WorksFor = @CompanyPK";

        SqlParameter[] companyParameters =
        [
            new SqlParameter("@CompanyPK", companyPK),
        ];

        List<EmployeeDto> employees = [];

        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, companyCommandText, CommandType.Text, companyParameters);

        while (reader.Read())
        {
            EmployeeDto employee = new()
            {
                EmpID = reader.GetGuid(reader.GetOrdinal("EmpID")),
                PersonPK = reader.GetGuid(reader.GetOrdinal("PersonPK")),
                WorksFor = reader.GetGuid(reader.GetOrdinal("WorksFor")),
                JobPosition = reader.GetString(reader.GetOrdinal("JobPosition")),
                ContractType = reader.GetString(reader.GetOrdinal("ContractType")),
            };

            employees.Add(employee);
        }

        foreach (var employee in employees)
        {
            const string personCommandText = @"
            SELECT
            TOP 1
                Id,
                Name,
                LastName
            FROM
                Persons
            WHERE
                PersonPK = @PersonPK";

            SqlParameter[] personParameters =
            [
                new SqlParameter("@PersonPK", employee.PersonPK)
            ];

            using SqlDataReader personReader = SqlHelper.ExecuteReader(this._connectionString, personCommandText, CommandType.Text, personParameters);

            if (personReader.Read())
            {
                employee.Id = personReader.GetString(personReader.GetOrdinal("Id"));
                employee.Name = personReader.GetString(personReader.GetOrdinal("Name"));
                employee.LastName = personReader.GetString(personReader.GetOrdinal("LastName"));
            }
        }

        return employees;
    }
}
