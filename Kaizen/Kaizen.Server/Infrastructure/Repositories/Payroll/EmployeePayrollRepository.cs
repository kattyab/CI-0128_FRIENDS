using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories.Payroll
{
    public class EmployeePayrollRepository : IEmployeePayrollRepository
    {
        private readonly IConfiguration _configuration;

        public EmployeePayrollRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<EmployeePayroll>> GetEmployeeDataAsync(Guid companyId)
        {
            var employeeData = new List<EmployeePayroll>();
            var connectionString = _configuration.GetConnectionString("KaizenDb");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = @"
                SELECT 
                    E.EmpID, E.BruteSalary, E.StartDate, E.FireDate, 
                    E.ContractType, E.RegistersHours,
                    dbo.GetPayrollTypeDescription(C.PayrollType) AS PayrollTypeDescription
                FROM dbo.Employees E
                INNER JOIN dbo.Companies C ON E.WorksFor = C.CompanyPK
                LEFT JOIN dbo.ApprovedHours AH ON E.EmpID = AH.EmpID AND AH.Status = 'Approved'
                WHERE 
                    C.CompanyPK = @CompanyID
                AND (E.IsDeleted = 0 OR E.IsDeleted IS NULL)
                AND (E.RegistersHours = 0 OR (E.RegistersHours = 1 AND AH.EmpID IS NOT NULL));";

            await using var command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@CompanyID", companyId.ToString());

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                employeeData.Add(MapEmployeeFromReader(reader));
            }

            return employeeData;
        }

        public async Task<Guid> GetPersonPkByEmailAsync(string email)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var cmdText = "SELECT PersonPK FROM dbo.Users WHERE Email = @Email";

            await using var command = new SqlCommand(cmdText, connection);
            command.Parameters.AddWithValue("@Email", email);

            var result = await command.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return (Guid)result;
            }

            throw new InvalidOperationException($"No user found with email: {email}");
        }

        private static EmployeePayroll MapEmployeeFromReader(SqlDataReader reader)
        {
            return new EmployeePayroll
            {
                EmpID = reader.GetGuid(0),
                BruteSalary = reader.GetDecimal(1),
                StartDate = reader.GetDateTime(2),
                FireDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                ContractType = reader.GetString(4),
                RegistersHours = reader.GetBoolean(5),
                PayrollTypeDescription = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
    }
}
