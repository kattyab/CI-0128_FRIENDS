using System.Collections.Generic;
using System.Threading.Tasks;
using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Reports
{
    public class EmployeePayrollListRepository : IEmployeePayrollListRepository
    {
        private readonly string? _connectionString;
        public EmployeePayrollListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb");
        }

        public async Task<List<EmployeePayrollListDto>> GetAllEmployeePayrollsAsync()
        {
            var result = new List<EmployeePayrollListDto>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var query = @"
                SELECT CONCAT(p.Name, ' ', p.LastName) AS EmployeeName, p.ID AS Cedula
                FROM Payrolls pr
                INNER JOIN Employees e ON pr.PaidTo = e.EmpID
                INNER JOIN Persons p ON e.PersonPK = p.PersonPK
            ";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new EmployeePayrollListDto
                {
                    EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
                    Cedula = reader["Cedula"].ToString() ?? string.Empty,
                    TipoEmpleado = string.Empty,
                    PeriodoPago = string.Empty,
                    FechaPago = string.Empty,
                    SalarioBruto = string.Empty,
                    CargasSociales = string.Empty,
                    DeduccionesVoluntarias = string.Empty,
                    CostoEmpleador = string.Empty
                });
            }
            return result;
        }
    }
}
