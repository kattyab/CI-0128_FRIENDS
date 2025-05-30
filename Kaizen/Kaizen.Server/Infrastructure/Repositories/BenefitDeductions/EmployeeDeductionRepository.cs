using Microsoft.Data.SqlClient;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class EmployeeDeductionRepository : IEmployeeDeductionRepository
    {
        private readonly SqlConnection _connection;

        public EmployeeDeductionRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public Dictionary<Guid, Employee> GetEmployeesByCompany(Guid companyID)
        {
            var employees = new Dictionary<Guid, Employee>();

            const string sql = @"
                SELECT EmpID, ContractType, StartDate, BruteSalary
                FROM dbo.Employees
                WHERE WorksFor = @CompanyID;
            ";

            using var cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CompanyID", companyID);

            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var empID = reader.GetGuid(0);
                employees[empID] = new Employee
                {
                    EmpID = empID,
                    ContractType = reader.GetString(1),
                    StartDate = reader.GetDateTime(2),
                    BruteSalary = reader.GetDecimal(3)
                };
            }
            return employees;
        }

        public Dictionary<Guid, List<Guid>> GetChosenBenefitsByCompany(Guid companyID)
        {
            var chosenBenefits = new Dictionary<Guid, List<Guid>>();

            const string sql = @"
                SELECT cb.EmployeeID, cb.BenefitID
                FROM dbo.ChosenBenefits cb
                INNER JOIN dbo.Employees e ON cb.EmployeeID = e.EmpID
                WHERE e.WorksFor = @CompanyID;
            ";

            using var cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CompanyID", companyID);

            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
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
