using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class EmployeesRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

    public List<EmployeeDto> GetEmployees()
    {
        const string commandText = @"
            SELECT
                EmpID,
                PersonPK,
                WorksFor,
                JobPosition,
                ContractType
            FROM
                Employees";

        List<EmployeeDto> employees = [];

        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, commandText, CommandType.Text);

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
            const string personQuery = @"
            SELECT
            TOP 1
                Id,
                Name,
                LastName
            FROM
                Persons
            WHERE
                PersonPK = @PersonPK";

            SqlParameter[] parameters =
            [
                new SqlParameter("@PersonPK", employee.PersonPK)
            ];

            using SqlDataReader personReader = SqlHelper.ExecuteReader(this._connectionString, personQuery, CommandType.Text, parameters);

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
