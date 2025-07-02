using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Kaizen.Server.Application.Dtos.OwnerDashboard;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class OwnerDashboardRepository
    {
        private readonly string _connectionString;

        public OwnerDashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException("La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public async Task<List<ContractCountResultDto>> GetContractCountsLast3MonthsAsync(Guid companyPk)
        {
            var now = DateTime.UtcNow;
            var months = new[]
            {
                new { Year = now.AddMonths(-2).Year, Month = now.AddMonths(-2).Month },
                new { Year = now.AddMonths(-1).Year, Month = now.AddMonths(-1).Month },
                new { Year = now.Year, Month = now.Month }
            };

            var results = new List<ContractCountResultDto>();
            const string sql = @"
                SELECT
                    ContractType,
                    @Year AS Year,
                    @Month AS Month,
                    COUNT(*) AS Count
                FROM Employees
                WHERE WorksFor = @companyPk
                  AND StartDate <= @MonthEnd
                  AND (FireDate IS NULL OR FireDate >= @MonthStart)
                GROUP BY ContractType
            ";

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                foreach (var m in months)
                {
                    var monthStart = new DateTime(m.Year, m.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@companyPk", companyPk);
                        cmd.Parameters.AddWithValue("@Year", m.Year);
                        cmd.Parameters.AddWithValue("@Month", m.Month);
                        cmd.Parameters.AddWithValue("@MonthStart", monthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new ContractCountResultDto
                                {
                                    ContractType = reader.GetString(reader.GetOrdinal("ContractType")),
                                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                    Month = reader.GetInt32(reader.GetOrdinal("Month")),
                                    Count = reader.GetInt32(reader.GetOrdinal("Count"))
                                });
                            }
                        }
                    }
                }
            }
            return results;
        }

        public async Task<List<LastPayrollDto>> GetLast3PayrollsAsync(Guid companyPk)
        {
            var results = new List<LastPayrollDto>();
            const string sql = @"
                SELECT TOP 3
                    Period,
                    ExecutedOn,
                    TotalMoneyPaid
                FROM GeneralPayrolls
                WHERE PaidBy = @companyPk
                ORDER BY ExecutedOn DESC
            ";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@companyPk", companyPk);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new LastPayrollDto
                        {
                            Period = reader["Period"]?.ToString() ?? string.Empty,
                            ExecutedOn = reader.GetDateTime(reader.GetOrdinal("ExecutedOn")),
                            TotalMoneyPaid = reader.GetDecimal(reader.GetOrdinal("TotalMoneyPaid"))
                        });
                    }
                }
            }
            return results;
        }

        public async Task<PayrollCostBreakdownDto?> GetPayrollCostBreakdownAsync(Guid companyPk)
        {
            const string sql = @"
                SELECT TOP 1
                    TotalDeductionsBenefits,
                    TotalObligatoryDeductions,
                    TotalLaborCharges,
                    TotalMoneyPaid
                FROM GeneralPayrolls
                WHERE PaidBy = @companyPk
                ORDER BY ExecutedOn DESC
            ";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@companyPk", companyPk);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var benefits = reader.GetDecimal(reader.GetOrdinal("TotalDeductionsBenefits"));
                        var obligatoryDeductions = reader.GetDecimal(reader.GetOrdinal("TotalObligatoryDeductions"));
                        var laborCharges = reader.GetDecimal(reader.GetOrdinal("TotalLaborCharges"));
                        var total = reader.GetDecimal(reader.GetOrdinal("TotalMoneyPaid"));
                        var salaries = total - (benefits + obligatoryDeductions + laborCharges);
                        return new PayrollCostBreakdownDto
                        {
                            Benefits = benefits,
                            ObligatoryDeductions = obligatoryDeductions,
                            LaborCharges = laborCharges,
                            Salaries = salaries
                        };
                    }
                }
            }
            return null;
        }
    }
}
