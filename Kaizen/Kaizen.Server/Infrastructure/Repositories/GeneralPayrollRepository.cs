using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Kaizen.Server.Application.Dtos.Payroll;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public sealed class GeneralPayrollRepository
    {
        private readonly SqlConnection _conn;
        public GeneralPayrollRepository(SqlConnection conn) => _conn = conn;

        public async Task<bool> ExistsPeriodAsync(Guid companyPk, char mode, string period)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM   dbo.GeneralPayrolls
                WHERE  PaidBy      = @company
                  AND  PayrollMode = @mode
                  AND  Period      = @period;";

            if (_conn.State != ConnectionState.Open)
                await _conn.OpenAsync();

            using var cmd = new SqlCommand(sql, _conn);
            cmd.Parameters.Add("@company", SqlDbType.UniqueIdentifier).Value = companyPk;
            cmd.Parameters.Add("@mode", SqlDbType.Char, 1).Value = mode;
            cmd.Parameters.Add("@period", SqlDbType.NVarChar, 25).Value = period;

            var count = (int)await cmd.ExecuteScalarAsync()!;
            return count > 0;
        }

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

        public async Task<IEnumerable<PayrollHistoryRowDto>> GetHistoryAsync()
        {
            const string sql = "EXEC dbo.GetPayrollHistory;";
            if (_conn.State != ConnectionState.Open)
                await _conn.OpenAsync();

            using var cmd = new SqlCommand(sql, _conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<PayrollHistoryRowDto>();
            while (await reader.ReadAsync())
            {
                list.Add(new PayrollHistoryRowDto
                {
                    Id = reader.GetGuid(0),
                    Manager = reader.GetString(1),
                    Type = reader.GetString(2),
                    Period = reader.GetString(3),
                    Deductions = reader.GetDecimal(4),
                    SocialCharges = reader.GetDecimal(5),
                    Total = reader.GetDecimal(6),
                    Gross = reader.GetDecimal(7),
                    Net = reader.GetDecimal(8)
                });
            }

            return list;
        }

        public async Task<IEnumerable<PayrollHistoryRowDto>> GetHistoryByCompanyAsync(Guid companyPk)
        {
            const string sql = @"
        SELECT
            gp.GeneralPayrollsID                                                   AS Id,
            gp.InCharge                                                            AS Manager,
            CASE gp.PayrollMode
                 WHEN 'W' THEN 'Semanal'
                 WHEN 'B' THEN 'Quincenal'
                 WHEN 'M' THEN 'Mensual'
                 ELSE 'Desconocido'
            END                                                                     AS [Type],
            gp.Period,
            (gp.TotalDeductionsBenefits + gp.TotalObligatoryDeductions)            AS Deductions,
            gp.TotalLaborCharges                                                    AS SocialCharges,
            gp.TotalMoneyPaid                                                       AS Total,
            (gp.TotalMoneyPaid - gp.TotalLaborCharges)                              AS Gross,
            (gp.TotalMoneyPaid - gp.TotalLaborCharges
             - (gp.TotalDeductionsBenefits + gp.TotalObligatoryDeductions))         AS Net
        FROM dbo.GeneralPayrolls gp
        WHERE gp.PaidBy = @companyPk
        ORDER BY gp.ExecutedOn DESC;";

            if (_conn.State != ConnectionState.Open)
                await _conn.OpenAsync();

            using var cmd = new SqlCommand(sql, _conn);
            cmd.Parameters.Add("@companyPk", SqlDbType.UniqueIdentifier).Value = companyPk;

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new List<PayrollHistoryRowDto>();

            while (await reader.ReadAsync())
            {
                list.Add(new PayrollHistoryRowDto
                {
                    Id = reader.GetGuid(0),
                    Manager = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Type = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Period = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Deductions = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4),
                    SocialCharges = reader.IsDBNull(5) ? 0m : reader.GetDecimal(5),
                    Total = reader.IsDBNull(6) ? 0m : reader.GetDecimal(6),
                    Gross = reader.IsDBNull(7) ? 0m : reader.GetDecimal(7),
                    Net = reader.IsDBNull(8) ? 0m : reader.GetDecimal(8)
                });
            }

            return list;
        }
    }
}
