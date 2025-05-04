using System.Data;
using System.Data.SqlClient;
using Kaizen.Server.Models;

namespace Kaizen.Server.Repository
{
    public class RolCambioHandler
    {
        private SqlConnection _conexion;
        private string _rutaConexion;

        public RolCambioHandler()
        {
            var builder = WebApplication.CreateBuilder();
            _rutaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
            _conexion = new SqlConnection(_rutaConexion);
        }

        // cambiar el rol de un usuario por su email
        public bool CambiarRolPorEmail(string email, string nuevoRol)
        {
            string query = "UPDATE Users SET Role = @Role WHERE Email = @Email";

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                command.Parameters.AddWithValue("@Role", nuevoRol);
                command.Parameters.AddWithValue("@Email", email);

                _conexion.Open();
                bool resultado = command.ExecuteNonQuery() == 1;
                _conexion.Close();

                return resultado;
            }
        }

        // obtener todos los usuarios y sus roles
        public List<RolCambioModel> ObtenerUsuarios()
        {
            var usuarios = new List<RolCambioModel>();
            string query = "SELECT Email, Role FROM Users"; 

            using (SqlCommand command = new SqlCommand(query, _conexion))
            {
                _conexion.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new RolCambioModel
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
