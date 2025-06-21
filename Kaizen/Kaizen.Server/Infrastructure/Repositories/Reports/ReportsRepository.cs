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
                    owner.LastName
                ORDER BY gp.ExecutedOn DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@CompanyId", companyId);

            return await ExecuteQueryAsync(command);
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
