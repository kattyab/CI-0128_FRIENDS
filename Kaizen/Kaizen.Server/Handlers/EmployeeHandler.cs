using System.Data;
using Microsoft.Data.SqlClient;

using Kaizen.Server.Models;

namespace Kaizen.Server.Handlers
{
    public class EmployeeHandler
    {
        private SqlConnection _connection;
        private string _connectionPath;

        public EmployeeHandler(IConfiguration configuration)
        {
            _connectionPath = configuration.GetConnectionString("EmployeeDetails");
            _connection = new SqlConnection(_connectionPath);
        }

        public DataTable CreateConsultTable(string query, SqlParameter[] parameters = null)
        {
            using var connection = new SqlConnection(_connection.ConnectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            var dataAdapter = new SqlDataAdapter(command);
            var resultTable = new DataTable();
            dataAdapter.Fill(resultTable);
            return resultTable;
        }

        public object ObtainEmployeeData(string email)
        {
            string consulta = @"
            SELECT 
                p.ID AS cedula, p.Name AS nombre, p.LastName AS apellido, p.Sex AS sexo, 
                p.BirthDate AS fecha_nacimiento, p.Province AS provincia, p.Canton AS canton, p.OtherSigns AS otras_senas,
                t.Number AS telefono, 
                e.BruteSalary AS salario_bruto, e.ContractType AS tipo_contrato, e.StartDate AS fecha_ingreso, 
                e.PayCycleType AS periodicidad, e.EmployeeNumber AS puesto, u.Role AS rol, u.Active AS estado,
                cb.BenefitName AS beneficio,
                u.Email AS correo
            FROM Users u
            LEFT JOIN Persons p ON u.PersonID = p.ID
            LEFT JOIN PersonPhoneNumbers t ON p.ID = t.PersonID
            LEFT JOIN Employees e ON p.ID = e.EmployeeID
            LEFT JOIN ChosenBenefits cb ON e.EmployeeID = cb.EmployeeID
            WHERE u.Email = @email;";

            SqlParameter[] sqlParameters = new[] {
                new SqlParameter("@Email", email)
            };

            DataTable employeeDataTable = CreateConsultTable(consulta, sqlParameters);

            if (employeeDataTable.Rows.Count == 0)
                return null;

            var employeeData = new
            {
                Cedula = Convert.ToString(employeeDataTable.Rows[0]["cedula"]),
                Nombre = Convert.ToString(employeeDataTable.Rows[0]["nombre"]),
                Apellido = Convert.ToString(employeeDataTable.Rows[0]["apellido"]),
                Sexo = Convert.ToString(employeeDataTable.Rows[0]["sexo"]),
                FechaNacimiento = Convert.ToDateTime(employeeDataTable.Rows[0]["fecha_nacimiento"]),
                Provincia = Convert.ToString(employeeDataTable.Rows[0]["provincia"]),
                Canton = Convert.ToString(employeeDataTable.Rows[0]["canton"]),
                OtrasSenas = Convert.ToString(employeeDataTable.Rows[0]["otras_senas"]),
                Telefonos = employeeDataTable.AsEnumerable()
                                 .Where(row => !row.IsNull("telefono"))
                                 .Select(row => Convert.ToString(row["telefono"]))
                                 .Distinct()
                                 .ToList(),
                SalarioBruto = employeeDataTable.Rows[0].IsNull("salario_bruto") ? (decimal?)null : Convert.ToDecimal(employeeDataTable.Rows[0]["salario_bruto"]),
                TipoContrato = Convert.ToString(employeeDataTable.Rows[0]["tipo_contrato"]),
                FechaIngreso = employeeDataTable.Rows[0].IsNull("fecha_ingreso") ? (DateTime?)null : Convert.ToDateTime(employeeDataTable.Rows[0]["fecha_ingreso"]),
                Periodicidad = Convert.ToString(employeeDataTable.Rows[0]["periodicidad"]),
                Puesto = Convert.ToString(employeeDataTable.Rows[0]["puesto"]),
                Rol = Convert.ToString(employeeDataTable.Rows[0]["rol"]),
                Estado = Convert.ToBoolean(employeeDataTable.Rows[0]["estado"]) ? "Activo" : "Inactivo",
                Beneficios = employeeDataTable.AsEnumerable()
                                  .Where(row => !row.IsNull("beneficio"))
                                  .Select(row => Convert.ToString(row["beneficio"]))
                                  .Distinct()
                                  .ToList(),
                Correo = Convert.ToString(employeeDataTable.Rows[0]["correo"])
            };

            return employeeData;
        }

    }
}
