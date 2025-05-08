using System.Data;
using Kaizen.Server.Models;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Repository;

public class NotificationsRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

    public List<NotificationDto> GetNotifications(Guid userPK)
    {
        const string commandText = @"
                SELECT
                    Id,
                    Description,
                    NotificationDate,
                    UserPK
                FROM
                    Notifications
                WHERE
                    UserPK = @UserPK
                ORDER BY
                    NotificationDate DESC";

        List<NotificationDto> notifications = [];

        SqlParameter[] parameters =
        [
            new SqlParameter("@UserPK", userPK),
        ];

        using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString, commandText, CommandType.Text, parameters);

        while (reader.Read())
        {
            NotificationDto notification = new()
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                NotificationDate = reader.GetDateTime(reader.GetOrdinal("NotificationDate")),
                UserPK = reader.GetGuid(reader.GetOrdinal("UserPK"))
            };

            notifications.Add(notification);
        }
        return notifications;
    }
}
