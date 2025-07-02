using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Reports
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly string _connectionString;

        public ReportsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb");
        }

        public async Task<IEnumerable<OwnerPayrollReport>> GetPayrollReportsByCompanyAsync(Guid companyId)
        {
            const string sql = @"
                SELECT 
                    gp.Period,
                    gp.ExecutedOn,
                    CONCAT(owner.Name, ' ', owner.LastName) AS OwnerFullName,
                    c.CompanyName,
                    gp.TotalLaborCharges,
                    gp.TotalMoneyPaid,
                    ISNULL(SUM(CASE WHEN e.ContractType = 'Servicios Profesionales' THEN pr.BrutePaid ELSE 0 END), 0) AS ServiciosProfesionalesAmount,
                    ISNULL(SUM(CASE WHEN e.ContractType = 'Por Horas' THEN pr.BrutePaid ELSE 0 END), 0) AS PorHorasAmount,
                    ISNULL(SUM(CASE WHEN e.ContractType IN ('Medio Tiempo', 'Tiempo Completo') THEN pr.BrutePaid ELSE 0 END), 0) AS TiempoCompletoAmount
                FROM GeneralPayrolls gp
                INNER JOIN Payrolls pr ON gp.GeneralPayrollsID = pr.GeneralPayrollPk
                INNER JOIN Employees e ON pr.PaidTo = e.EmpID
                INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
                INNER JOIN Persons owner ON c.OwnerPK = owner.PersonPK
                WHERE c.CompanyPK = @CompanyId
                GROUP BY 
                    gp.Period,
                    gp.ExecutedOn,
                    gp.TotalLaborCharges,
                    gp.TotalMoneyPaid,
                    owner.Name,
                    owner.LastName,
                    c.CompanyName
                ORDER BY gp.ExecutedOn DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CompanyId", companyId);

            return await ExecuteQueryAsync(command);
        }

        public async Task<IEnumerable<EmployeePayrollReport>> GetEmployeePayrollReportsByEmployeeAsync(Guid employeeId)
        {
            const string sql = @"
                SELECT 
                    pr.PayrollID,
                    gp.Period,
                    c.CompanyName,
                    CONCAT(employee.Name, ' ', employee.LastName) AS EmployeeFullName,
                    e.ContractType,
                    gp.ExecutedOn,
                    pr.IncomeTax,
                    pr.NetPaid,
                    ISNULL(SUM(CASE WHEN e.ContractType = 'Servicios Profesionales' THEN pr.BrutePaid ELSE 0 END), 0) AS ServiciosProfesionalesAmount,
                    ISNULL(SUM(CASE WHEN e.ContractType = 'Por Horas' THEN pr.BrutePaid ELSE 0 END), 0) AS PorHorasAmount,
                    ISNULL(SUM(CASE WHEN e.ContractType IN ('Medio Tiempo', 'Tiempo Completo') THEN pr.BrutePaid ELSE 0 END), 0) AS TiempoCompletoAmount,
                    pr.BrutePaid
                FROM GeneralPayrolls gp
                INNER JOIN Payrolls pr ON gp.GeneralPayrollsID = pr.GeneralPayrollPk
                INNER JOIN Employees e ON pr.PaidTo = e.EmpID
                INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
                INNER JOIN Persons employee ON e.PersonPK = employee.PersonPK
                WHERE e.EmpId = @EmployeeId
                GROUP BY 
                    pr.PayrollID,
                    e.ContractType,
                    pr.IncomeTax,
                    pr.NetPaid,
                    gp.Period,
                    gp.ExecutedOn,
                    employee.Name,
                    employee.LastName,
                    c.CompanyName,
                    pr.BrutePaid
                ORDER BY gp.ExecutedOn DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);

            return await ExecuteEmployeeQueryAsync(command);
        }

        public async Task<IEnumerable<OptionalDeduction>> GetOptionalDeductionsByPayrollAsync(Guid payrollId)
        {
            const string sql = @"
                SELECT Name, Amount 
                FROM OptionalDeductions
                WHERE PayrollId = @PayrollId
                GROUP BY Name, Amount
                ORDER BY Name";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@PayrollId", payrollId);

            return await ExecuteOptionalDeductionsQueryAsync(command);
        }

        private async Task<IEnumerable<EmployeePayrollReport>> ExecuteEmployeeQueryAsync(SqlCommand command)
        {
            var results = new List<EmployeePayrollReport>();
            await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new EmployeePayrollReport
                {
                    PayrollID = reader.GetGuid("PayrollID"),
                    Period = reader.GetString("Period"),
                    CompanyName = reader.GetString("CompanyName"),
                    EmployeeFullName = reader.GetString("EmployeeFullName"),
                    ContractType = reader.GetString("ContractType"),
                    PayDate = reader.GetDateTime("ExecutedOn"),
                    BruteSalary = reader.GetDecimal("BrutePaid"),
                    IncomeTax = reader.GetDecimal("IncomeTax"),
                    NetPay = reader.GetDecimal("NetPaid")
                });
            }

            return results;
        }

        private async Task<IEnumerable<OptionalDeduction>> ExecuteOptionalDeductionsQueryAsync(SqlCommand command)
        {
            var results = new List<OptionalDeduction>();
            await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new OptionalDeduction
                {
                    Name = reader.GetString("Name"),
                    Amount = reader.GetDecimal("Amount")
                });
            }

            return results;
        }
        private async Task<IEnumerable<OwnerPayrollReport>> ExecuteQueryAsync(SqlCommand command)
        {
            var results = new List<OwnerPayrollReport>();

            await command.Connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new OwnerPayrollReport
                {
                    Period = reader.GetString("Period"),
                    ExecutedOn = reader.GetDateTime("ExecutedOn"),
                    OwnerFullName = reader.GetString("OwnerFullName"),
                    CompanyName = reader.GetString("CompanyName"),
                    TotalLaborCharges = reader.GetDecimal("TotalLaborCharges"),
                    TotalMoneyPaid = reader.GetDecimal("TotalMoneyPaid"),
                    ServiciosProfesionalesAmount = reader.GetDecimal("ServiciosProfesionalesAmount"),
                    PorHorasAmount = reader.GetDecimal("PorHorasAmount"),
                    TiempoCompletoAmount = reader.GetDecimal("TiempoCompletoAmount")
                });
            }

            return results;
        }
    }
}
