using Kaizen.Server.Application.Dtos;
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
FROM 
    Employees e
INNER JOIN 
    Companies c ON e.WorksFor = c.CompanyPK
INNER JOIN 
    Persons p ON e.PersonPK = p.PersonPK
INNER JOIN 
    Users u ON u.Email = @Email
LEFT JOIN 
    Persons owner ON c.OwnerPK = owner.PersonPK AND u.PersonPK = owner.PersonPK AND u.Role = 'Dueño'
LEFT JOIN 
    Admins a ON a.CompanyPK = c.CompanyPK AND a.AdminPK = u.PersonPK AND u.Role = 'Administrador'
WHERE 
    (
        (u.Role = 'Dueño' AND c.OwnerPK = u.PersonPK) OR
        (u.Role = 'Administrador' AND a.AdminPK IS NOT NULL)
    );", connection);

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
