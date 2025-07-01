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
                SELECT 
                    CONCAT(p.Name, ' ', p.LastName) AS EmployeeName, 
                    p.ID AS Cedula, 
                    e.ContractType, 
                    pr.BrutePaid,
                    gp.Period, 
                    gp.ExecutedOn
                FROM Payrolls pr
                INNER JOIN Employees e ON pr.PaidTo = e.EmpID
                INNER JOIN Persons p ON e.PersonPK = p.PersonPK
                INNER JOIN GeneralPayrolls gp ON pr.GeneralPayrollPk = gp.GeneralPayrollsID
            ";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string salarioBruto = "0";
                if (reader["BrutePaid"] != DBNull.Value)
                {
                    salarioBruto = ((decimal)reader["BrutePaid"]).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                }
                string periodoPago = string.Empty;
                if (reader["Period"] != DBNull.Value)
                {
                    periodoPago = reader["Period"].ToString() ?? string.Empty;
                }
                string fechaPago = string.Empty;
                if (reader["ExecutedOn"] != DBNull.Value)
                {
                    // Si quieres formatear la fecha, puedes hacerlo aquí
                    var fecha = (DateTime)reader["ExecutedOn"];
                    fechaPago = fecha.ToString("yyyy-MM-dd");
                }
                result.Add(new EmployeePayrollListDto
                {
                    EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
                    Cedula = reader["Cedula"].ToString() ?? string.Empty,
                    TipoEmpleado = reader["ContractType"].ToString() ?? string.Empty,
                    PeriodoPago = periodoPago,
                    FechaPago = fechaPago,
                    SalarioBruto = salarioBruto,
                    CargasSociales = string.Empty,
                    DeduccionesVoluntarias = string.Empty,
                    CostoEmpleador = string.Empty
                });
            }
            return result;
        }
    }
}
