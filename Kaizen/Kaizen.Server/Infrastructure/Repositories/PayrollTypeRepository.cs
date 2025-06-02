// Archivo: Kaizen.Server.Infrastructure.Repositories\PayrollRepository.cs
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class PayrollRepository
    {
        private readonly string _connectionString;

        public PayrollRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string? GetPayrollTypeByUserId(Guid userId)
        {
            const string sql = @"
                SELECT c.PayrollType
                FROM   Users     u
                JOIN   Companies c ON c.CompanyPK = u.CompanyPK
                WHERE  u.UserPK = @UserId;
            ";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = userId });

            var table = new DataTable();
            new SqlDataAdapter(cmd).Fill(table);

            if (table.Rows.Count == 0) return null;
            return table.Rows[0]["PayrollType"]?.ToString();
        }
    }
}
