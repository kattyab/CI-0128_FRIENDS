using System.Data;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Handlers   //  coincide con la carpeta /Handlers
{
    public class CredencialesH
    {
        private readonly string _connectionString; 

        public CredencialesH(IConfiguration configuration)
        {
            // Si la clave no existe arroja una excepción clara
            _connectionString = configuration.GetConnectionString("EmployeeDetails")
                ?? throw new InvalidOperationException(
                     "La cadena de conexión 'EmployeeDetails' no está definida en appsettings.json.");
        }

        // util para ejecutar cualquier consulta y devolver DataTable
        private static DataTable ExecuteQuery(string sql,
                                              string connectionString,
                                              SqlParameter[]? parameters = null)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(sql, connection);

            if (parameters is not null)
                command.Parameters.AddRange(parameters);

            var table = new DataTable();
            new SqlDataAdapter(command).Fill(table);
            return table;
        }

        /// <summary>
        /// </summary>
        public object? ObtainUserData(string email)  
        {
            const string sql = @"
                SELECT  Email,
                        Password,
                        Active,
                        Role,
                        PersonID
                FROM    Users
                WHERE   Email = @Email;";

            var parametros = new[]
            {
                new SqlParameter("@Email", email ?? string.Empty) // evita CS8625
            };

            DataTable dt = ExecuteQuery(sql, _connectionString, parametros);

            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];

            return new
            {
                Email = r["Email"]?.ToString(),
                Password = r["Password"]?.ToString(),        // sólo para pruebas
                Active = Convert.ToBoolean(r["Active"]),
                Role = r["Role"]?.ToString(),
                PersonID = r["PersonID"]?.ToString()
            };
        }
    }
}
