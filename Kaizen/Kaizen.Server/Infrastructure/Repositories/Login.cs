using System.Data;
using Kaizen.Server.Application.Dtos.Auth;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class Login(IConfiguration configuration)
    {
        private readonly string _connectionString =
            configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException(
                   "The connection string 'KaizenDb' is not defined in appsettings.json.");

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

        public object? ObtainUserData(string email)
        {
            const string sql = @"
                SELECT  Email,
                        PasswordHash,
                        Active,
                        Role,
                        PersonPK,
                        UserPK
                FROM    Users
                WHERE   Email = @Email;";

            var parameters = new[]
            {
                new SqlParameter("@Email", email ?? string.Empty)
            };

            DataTable table = ExecuteQuery(sql, _connectionString, parameters);

            if (table.Rows.Count == 0) return null;

            DataRow row = table.Rows[0];

            return new
            {
                Email = row["Email"]?.ToString(),
                PasswordHash = row["PasswordHash"]?.ToString(),
                Active = Convert.ToBoolean(row["Active"]),
                Role = row["Role"]?.ToString(),
                PersonPK = row["PersonPK"]?.ToString(),
                UserPK = row["UserPK"]?.ToString()
            };
        }

        public AuthUserDto GetAuthUser(string email)
        {
            const string sql = @"
                SELECT  Email,
                        PasswordHash,
                        Active,
                        Role,
                        PersonPK,
                        UserPK
                FROM    Users
                WHERE   Email = @Email;";

            var parameters = new[]
            {
                new SqlParameter("@Email", email ?? string.Empty)
            };

            DataTable table = ExecuteQuery(sql, _connectionString, parameters);

            if (table.Rows.Count == 0)
            {
                throw new UnauthorizedAccessException($"User with email '{email}' not found.");
            }

            DataRow row = table.Rows[0];

            return new AuthUserDto()
            {
                Email = (string)row["Email"],
                Active = Convert.ToBoolean(row["Active"]),
                Role = (string)row["Role"],
                PersonPK = (Guid)row["PersonPK"],
                UserPK = (Guid)row["UserPK"],
            };
        }
    }
}
