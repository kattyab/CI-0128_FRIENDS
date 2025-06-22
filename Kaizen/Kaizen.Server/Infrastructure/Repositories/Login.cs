using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

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
            {
                command.Parameters.AddRange(parameters);
            }

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
        
        public Guid GetAuthUserCompanyPK(AuthUserDto authUser)
        {
            string companyPkCommandText = authUser.Role switch
            {
                "Administrador" => @"
                    SELECT A.CompanyPK
                    FROM Admins A
                    JOIN Users U ON A.AdminPK = U.PersonPK
                    WHERE U.UserPK = @UserPK;",
                "Dueño" => @"
                    SELECT C.CompanyPK
                    FROM Companies C
                    JOIN Persons P ON C.OwnerPK = P.PersonPK
                    JOIN Users U ON P.PersonPK= U.PersonPK
                    WHERE U.UserPK = @UserPK;",
                _ => throw new Exception("Invalid role for retrieving company.")
            };

            SqlParameter[] companyPKParameters = [
                new SqlParameter("@UserPK", authUser.UserPK),
            ];

            object companyPK = SqlHelper.ExecuteScalar(this._connectionString,
                companyPkCommandText,
                CommandType.Text,
                companyPKParameters);

            if (companyPK == null)
            {
                throw new Exception("Could not find company");
            }

            return (Guid)companyPK;
        }

        public Guid GetAuthUserEmployeePK(AuthUserDto authUser)
        {
            if (IsUnauthorizedRole(authUser))
            {
                throw new Exception("User is not an employee.");
            }
            const string commandText = @"
                SELECT E.EmpID
                FROM Employees E
                JOIN Users U ON E.PersonPK = U.PersonPK
                WHERE U.UserPK = @UserPK;";

            SqlParameter[] parameters = [
                new SqlParameter("@UserPK", authUser.UserPK),
             ];

            object empIdObj = SqlHelper.ExecuteScalar(this._connectionString,
                commandText,
                CommandType.Text,
                parameters);

            if (empIdObj == null || empIdObj == DBNull.Value)
            {
                throw new Exception("Employee not found for this user.");
            }

            return (Guid)empIdObj;
        }

        private static readonly HashSet<string> AuthorizedRoles = new()
        {
            "Empleado",
            "Administrador",
            "Supervisor"
        };

        private static bool IsUnauthorizedRole(AuthUserDto authUser)
        {
            Console.WriteLine(authUser.Role);
            return !AuthorizedRoles.Contains(authUser.Role);
        }
    }
}
