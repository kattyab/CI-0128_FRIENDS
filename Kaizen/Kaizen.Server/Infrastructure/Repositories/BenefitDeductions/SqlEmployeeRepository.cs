using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories.BenefitDeductions
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly SqlConnection _connection;

        public SqlEmployeeRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public Employee GetById(Guid employeeID)
        {
            using var conn = new SqlConnection(_connection.ConnectionString);
            conn.Open();

            const string sql = @"SELECT EmpID, ContractType, StartDate, BruteSalary, WorksFor FROM dbo.Employees WHERE EmpID = @empID";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empID", employeeID);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) throw new Exception("Employee not found.");

            return new Employee
            {
                EmpID = reader.GetGuid(0),
                ContractType = reader.GetString(1),
                StartDate = reader.GetDateTime(2),
                BruteSalary = reader.GetDecimal(3),
                WorksFor = reader.GetGuid(4)
            };
        }

        public List<Guid> GetChosenBenefitIDs(Guid employeeID)
        {
            var result = new List<Guid>();
            using var conn = new SqlConnection(_connection.ConnectionString);
            conn.Open();

            const string sql = @"SELECT BenefitID FROM dbo.ChosenBenefits WHERE EmployeeID = @empID";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empID", employeeID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetGuid(0));

            return result;
        }
    }
}
