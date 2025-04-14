using System.Data;
using Microsoft.Data.SqlClient;

namespace KaizenProto.Server.Handlers
{
    public class EmpleadoHandler
    {
        private SqlConnection _conexion;
        private string _rutaConexion;

        public EmpleadoHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _rutaConexion = builder.Configuration.GetConnectionString("EmployeeDetails");
            _conexion = new SqlConnection(_rutaConexion);
        }

        public DataTable CrearTablaConsulta(string consulta, SqlParameter[] parametros = null)
        {
            using var conexion = new SqlConnection(_conexion.ConnectionString);
            using var comando = new SqlCommand(consulta, conexion);

            if (parametros != null)
            {
                comando.Parameters.AddRange(parametros);
            }

            var adaptador = new SqlDataAdapter(comando);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public object ObtenerEmpleadoInfo(string cedula)
        {
            string consulta = @"
                SELECT p.cedula, p.nombre, p.apellido, p.sexo, p.fecha_nacimiento, p.provincia, p.canton, p.otras_senas,
                       t.telefono, e.salario_bruto, e.tipo_contrato, e.fecha_ingreso, e.periodicidad, e.puesto, e.rol, e.estado,
                       b.beneficio, u.correo
                FROM Personas p
                LEFT JOIN Telefonos t ON p.cedula = t.cedula_persona
                LEFT JOIN Empleados e ON p.cedula = e.cedula
                LEFT JOIN Beneficios b ON e.cedula = b.cedula_empleado
                LEFT JOIN Usuarios u ON p.cedula = u.cedula
                WHERE p.cedula = @cedula";

            SqlParameter[] parametros = new[] {
                new SqlParameter("@cedula", cedula)
            };

            DataTable tabla = CrearTablaConsulta(consulta, parametros);

            if (tabla.Rows.Count == 0)
                return null;

            var empleado = new
            {
                Cedula = Convert.ToString(tabla.Rows[0]["cedula"]),
                Nombre = Convert.ToString(tabla.Rows[0]["nombre"]),
                Apellido = Convert.ToString(tabla.Rows[0]["apellido"]),
                Sexo = Convert.ToString(tabla.Rows[0]["sexo"]),
                FechaNacimiento = Convert.ToDateTime(tabla.Rows[0]["fecha_nacimiento"]),
                Provincia = Convert.ToString(tabla.Rows[0]["provincia"]),
                Canton = Convert.ToString(tabla.Rows[0]["canton"]),
                OtrasSenas = Convert.ToString(tabla.Rows[0]["otras_senas"]),
                Telefonos = tabla.AsEnumerable()
                                 .Where(row => !row.IsNull("telefono"))
                                 .Select(row => Convert.ToString(row["telefono"]))
                                 .Distinct()
                                 .ToList(),
                SalarioBruto = tabla.Rows[0].IsNull("salario_bruto") ? (decimal?)null : Convert.ToDecimal(tabla.Rows[0]["salario_bruto"]),
                TipoContrato = Convert.ToString(tabla.Rows[0]["tipo_contrato"]),
                FechaIngreso = tabla.Rows[0].IsNull("fecha_ingreso") ? (DateTime?)null : Convert.ToDateTime(tabla.Rows[0]["fecha_ingreso"]),
                Periodicidad = Convert.ToString(tabla.Rows[0]["periodicidad"]),
                Puesto = Convert.ToString(tabla.Rows[0]["puesto"]),
                Rol = Convert.ToString(tabla.Rows[0]["rol"]),
                Estado = Convert.ToString(tabla.Rows[0]["estado"]),
                Beneficios = tabla.AsEnumerable()
                                  .Where(row => !row.IsNull("beneficio"))
                                  .Select(row => Convert.ToString(row["beneficio"]))
                                  .Distinct()
                                  .ToList(),
                Correo = Convert.ToString(tabla.Rows[0]["correo"])
            };

            return empleado;
        }
    }
}
