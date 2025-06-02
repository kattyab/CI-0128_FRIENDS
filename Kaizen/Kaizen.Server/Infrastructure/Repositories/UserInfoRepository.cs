using System.Data;
using Kaizen.Server.Application.Dtos.Auth;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class UserInfoRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException("Missing DB connection.");

    public UserInfoDto? GetUserInfo(Guid userPK)
    {
        const string query = @"
            SELECT 
                u.UserPK,
                e.RegistersHours,
                e.EmpID,
                c.PayrollType,
                e.StartDate,
                p.Name,
                p.LastName
            FROM Users u
            LEFT JOIN Persons p ON u.PersonPK = p.PersonPK
            LEFT JOIN Employees e ON p.PersonPK = e.PersonPK
            LEFT JOIN Companies c ON e.WorksFor = c.CompanyPK
            WHERE u.UserPK = @UserPK;
        ";
        using SqlDataReader reader = SqlHelper.ExecuteReader(
            _connectionString, query, CommandType.Text,
            new SqlParameter("@UserPK", userPK)
        );

        if (reader.Read())
        {
            return new UserInfoDto
            {
                UserPK = reader.GetGuid(reader.GetOrdinal("UserPK")),
                EmpID = reader.GetGuid(reader.GetOrdinal("EmpID")),
                RegistersHours = reader.IsDBNull(reader.GetOrdinal("RegistersHours")) ? null : reader.GetBoolean(reader.GetOrdinal("RegistersHours")),
                PayrollType = reader.IsDBNull(reader.GetOrdinal("PayrollType")) ? null : reader.GetString(reader.GetOrdinal("PayrollType")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
            };
        }

        return null;
    }

}
