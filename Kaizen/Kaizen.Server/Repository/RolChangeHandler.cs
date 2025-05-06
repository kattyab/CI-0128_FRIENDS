using System.Data;
using Microsoft.Data.SqlClient;
using Kaizen.Server.Models;

namespace Kaizen.Server.Repository
{
    // Handles role change operations related to the Users table
    public class RolChangeHandler
    {
        private SqlConnection _conexion;
        private string _rutaConexion;

        // Constructor initializes the SQL connection using configuration settings
        public RolChangeHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _rutaConexion = builder.Configuration.GetConnectionString("KaizenDb");
            _conexion = new SqlConnection(_rutaConexion);
        }

        // Updates the role of a user based on their email
        public bool CambiarRolPorEmail(string email, string nuevoRol)
        {
            string query = "UPDATE Users SET Role = @Role WHERE Email = @Email";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                // Bind parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Role", nuevoRol);
                command.Parameters.AddWithValue("@Email", email);

                _conexion.Open();
                bool resultado = command.ExecuteNonQuery() == 1;
                _conexion.Close();

                return resultado;
            }
        }

        // Retrieves all users and their roles from the database
        public List<RolChangeModel> ObtenerUsuarios()
        {
            var usuarios = new List<RolChangeModel>();
            string query = "SELECT Email, Role FROM Users";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                _conexion.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new RolChangeModel
                    {
                        Email = reader["Email"].ToString(),
                        NuevoRol = reader["Role"].ToString()
                    });
                }

                _conexion.Close();
            }

            return usuarios;
        }
    }
}
