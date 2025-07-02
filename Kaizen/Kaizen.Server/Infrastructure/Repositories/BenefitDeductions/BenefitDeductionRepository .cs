using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.BenefitDeductions
{
    public class BenefitDeductionRepository : IBenefitDeductionRepository
    {
        private readonly SqlConnection _connection;

        public BenefitDeductionRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        private static async Task EnsureOpenAsync(SqlConnection connection, string label)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();
        }

        public async Task<List<Benefit>> GetBenefitsByCompanyAsync(Guid companyID, PayrollTransactionContext context = null)
        {
            var connectionToUse = context?.Connection ?? _connection;
            string label = context?.Connection != null ? "PayrollTransactionContext" : "Default Repository Connection";
            await EnsureOpenAsync(connectionToUse, label);

            var benefits = new List<Benefit>();
            const string sql = @"
                SELECT 
                    ID, Name, MinWorkDurationMonths, IsFixed, FixedValue,
                    IsPercentage, PercentageValue, IsFullTime, IsPartTime, IsByHours, IsByService
                FROM dbo.Benefits
                WHERE OfferedBy = @CompanyID AND IsAPI = 0;
            ";

            using var command = context != null
                ? new SqlCommand(sql, connectionToUse, context.Transaction)
                : new SqlCommand(sql, connectionToUse);

            command.Parameters.AddWithValue("@CompanyID", companyID);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                benefits.Add(new Benefit
                {
                    BenefitID = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    MinWorkDurationMonths = reader.GetInt32(2),
                    IsFixed = reader.GetBoolean(3),
                    FixedValue = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    IsPercetange = reader.GetBoolean(5),
                    PercentageValue = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                    IsFullTime = reader.GetBoolean(7),
                    IsPartTime = reader.GetBoolean(8),
                    IsByHours = reader.GetBoolean(9),
                    IsByService = reader.GetBoolean(10)
                });
            }

            return benefits;
        }
    }
}
