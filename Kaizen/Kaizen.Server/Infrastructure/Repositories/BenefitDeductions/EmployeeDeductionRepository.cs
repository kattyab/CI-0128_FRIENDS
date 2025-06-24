using Microsoft.Data.SqlClient;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Contexts;
using System.Diagnostics;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class EmployeeDeductionRepository : IEmployeeDeductionRepository
    {
        private readonly SqlConnection _connection;

        public EmployeeDeductionRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        private static async Task EnsureOpenAsync(SqlConnection connection, string label)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

#if DEBUG
            Debug.WriteLine($"[DEBUG] Using connection from: {label}");
#endif
        }

        public async Task<Dictionary<Guid, EmployeeDto>> GetEmployeesByCompanyAsync(Guid companyID, PayrollTransactionContext context = null)
        {
            var employees = new Dictionary<Guid, EmployeeDto>();
            var connectionToUse = context?.Connection ?? _connection;
            string label = context != null ? "PayrollTransactionContext" : "Default Repository Connection";
            await EnsureOpenAsync(connectionToUse, label);

            const string sql = @"
                SELECT EmpID, StartDate, BruteSalary
                FROM dbo.Employees
                WHERE WorksFor = @CompanyID;
            ";

            using var command = context != null
                ? new SqlCommand(sql, connectionToUse, context.Transaction)
                : new SqlCommand(sql, connectionToUse);

            command.Parameters.AddWithValue("@CompanyID", companyID);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var empID = reader.GetGuid(0);
                employees[empID] = new EmployeeDto
                {
                    EmpID = empID,
                    StartDate = reader.GetDateTime(1),
                    BruteSalary = reader.GetDecimal(2)
                };
            }

            return employees;
        }

        public async Task<Dictionary<Guid, List<Guid>>> GetChosenBenefitsByCompanyAsync(Guid companyID, PayrollTransactionContext context = null)
        {
            var chosenBenefits = new Dictionary<Guid, List<Guid>>();
            var connectionToUse = context?.Connection ?? _connection;
            string label = context != null ? "PayrollTransactionContext" : "Default Repository Connection";
            await EnsureOpenAsync(connectionToUse, label);

            const string sql = @"
                SELECT cb.EmployeeID, cb.BenefitID
                FROM dbo.ChosenBenefits cb
                INNER JOIN dbo.Employees e ON cb.EmployeeID = e.EmpID
                WHERE e.WorksFor = @CompanyID;
            ";

            using var command = context != null
                ? new SqlCommand(sql, connectionToUse, context.Transaction)
                : new SqlCommand(sql, connectionToUse);

            command.Parameters.AddWithValue("@CompanyID", companyID);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var employeeID = reader.GetGuid(0);
                var benefitID = reader.GetGuid(1);

                if (!chosenBenefits.ContainsKey(employeeID))
                    chosenBenefits[employeeID] = new List<Guid>();

                chosenBenefits[employeeID].Add(benefitID);
            }

            return chosenBenefits;
        }
    }
}
