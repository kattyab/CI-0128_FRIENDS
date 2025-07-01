using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class GeneralPayrollReportRepository : IGeneralPayrollReportRepository
{
    private readonly string _connectionString;

    public GeneralPayrollReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException("Missing connection string: 'KaizenDb'.");
    }

    public List<GeneralPayrollReportDto> GetAllReports()
    {
        const string query = @"
            SELECT 
                gp.GeneralPayrollsID,
                gp.PaidBy,
                gp.TotalDeductionsBenefits,
                gp.TotalObligatoryDeductions,
                gp.TotalLaborCharges,
                gp.TotalMoneyPaid,
                gp.ExecutedOn,
                gp.PayrollMode,
                gp.Period,
                gp.InCharge,
                c.CompanyName
            FROM GeneralPayrolls gp
            LEFT JOIN Companies c ON gp.PaidBy = c.CompanyPK";

        var results = new List<GeneralPayrollReportDto>();

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, query, CommandType.Text);
        while (reader.Read())
        {
            results.Add(new GeneralPayrollReportDto
            {
                GeneralPayrollsID = reader.GetGuid(reader.GetOrdinal("GeneralPayrollsID")),
                PaidBy = reader.GetGuid(reader.GetOrdinal("PaidBy")),
                TotalDeductionsBenefits = reader.GetDecimal(reader.GetOrdinal("TotalDeductionsBenefits")),
                TotalObligatoryDeductions = reader.GetDecimal(reader.GetOrdinal("TotalObligatoryDeductions")),
                TotalLaborCharges = reader.GetDecimal(reader.GetOrdinal("TotalLaborCharges")),
                TotalMoneyPaid = reader.GetDecimal(reader.GetOrdinal("TotalMoneyPaid")),
                ExecutedOn = reader.GetDateTime(reader.GetOrdinal("ExecutedOn")),
                PayrollMode = reader.GetString(reader.GetOrdinal("PayrollMode")),
                Period = reader.GetString(reader.GetOrdinal("Period")),
                InCharge = reader.GetString(reader.GetOrdinal("InCharge")),
                CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CompanyName")),
            });
        }

        return results;
    }
}
