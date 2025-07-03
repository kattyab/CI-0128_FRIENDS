using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Helpers;

public static class SqlHelper
{
    public static int ExecuteNonQuery(string connectionString, string commandText,
        CommandType commandType, params SqlParameter[] parameters)
    {
        using SqlConnection conn = new(connectionString);
        using SqlCommand cmd = new(commandText, conn);
        cmd.CommandType = commandType;
        cmd.Parameters.AddRange(parameters);

        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    public static object ExecuteScalar(string connectionString, string commandText,
        CommandType commandType, params SqlParameter[] parameters)
    {
        using SqlConnection conn = new(connectionString);
        using SqlCommand cmd = new(commandText, conn);
        cmd.CommandType = commandType;
        cmd.Parameters.AddRange(parameters);

        conn.Open();
        return cmd.ExecuteScalar();
    }

    public static SqlDataReader ExecuteReader(string connectionString, string commandText,
        CommandType commandType, params SqlParameter[] parameters)
    {
        SqlConnection conn = new(connectionString);

        using SqlCommand cmd = new(commandText, conn);
        cmd.CommandType = commandType;
        cmd.Parameters.AddRange(parameters);

        conn.Open();
        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        return reader;
    }
}
