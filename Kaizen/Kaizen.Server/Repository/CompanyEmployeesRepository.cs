using Kaizen.Server.Models;
using Microsoft.Data.SqlClient;

public class CompanyEmployeesRepository
{
    private readonly string _connectionString;

    public CompanyEmployeesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KaizenDb");
    }

    public async Task<List<CompanyEmployeeSummaryDto>> GetEmployeesByOwnerEmail(string email)
    {
        var employees = new List<CompanyEmployeeSummaryDto>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            SELECT 
                e.EmpID,
                p.Name,
                p.LastName,
                p.Id,
                e.JobPosition,
                e.ContractType
            FROM Employees e
            INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
            INNER JOIN Persons p ON e.PersonPK = p.PersonPK
            INNER JOIN Persons owner ON c.OwnerPK = owner.PersonPK
            INNER JOIN Users u ON u.PersonPK = owner.PersonPK
            WHERE u.Email = @Email;", connection);

        command.Parameters.AddWithValue("@Email", email);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(new CompanyEmployeeSummaryDto
            {
                EmpID = reader["EmpID"].ToString(),
                Name = reader["Name"].ToString(),
                LastName = reader["LastName"].ToString(),
                Id = reader["Id"].ToString(),
                JobPosition = reader["JobPosition"]?.ToString(),
                ContractType = reader["ContractType"].ToString()
            });
        }

        return employees;
    }
}
