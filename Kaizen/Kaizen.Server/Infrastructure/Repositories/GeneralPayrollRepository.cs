using System.Data;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public sealed class GeneralPayrollRepository
{
    private readonly SqlConnection _conn;
    public GeneralPayrollRepository(SqlConnection conn) => _conn = conn;

    /// <summary>
    /// Actualiza la última planilla con PayrollMode, Period e InCharge.
    /// </summary>
    public async Task SetExtraFieldsAsync(char mode, string period, string inCharge)
    {
        const string sql = @"
UPDATE  gp
SET     gp.PayrollMode = @mode,
        gp.Period      = @period,
        gp.InCharge    = @inCharge
FROM    dbo.GeneralPayrolls gp
WHERE   gp.GeneralPayrollsID = (
            SELECT TOP 1 GeneralPayrollsID
            FROM   dbo.GeneralPayrolls
            ORDER BY ExecutedOn DESC
        );";

        if (_conn.State != ConnectionState.Open)
            await _conn.OpenAsync();

        using var cmd = new SqlCommand(sql, _conn);
        cmd.Parameters.Add("@mode", SqlDbType.Char, 1).Value = mode;
        cmd.Parameters.Add("@period", SqlDbType.NVarChar, 25).Value = period;
        cmd.Parameters.Add("@inCharge", SqlDbType.NVarChar, 150).Value = inCharge;

        await cmd.ExecuteNonQueryAsync();
    }
}
