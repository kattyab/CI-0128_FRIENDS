
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server;

public static class SqlHelper
{
    // Set the connection, command, and then execute the command with non query.
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

    // Set the connection, command, and then execute the command and only return one value.
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

    // Set the connection, command, and then execute the command with query and return the reader.
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

