using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories.BenefitDeductions
{
    public class BenefitDeductionRepository : IBenefitDeductionRepository
    {
        private readonly SqlConnection _connection;

        public BenefitDeductionRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Benefit> GetNonApiBenefitsByCompany(Guid companyID)
        {
            var benefits = new List<Benefit>();

            const string sql = @"
                SELECT 
                    ID, Name, MinWorkDurationMonths, IsFixed, FixedValue,
                    IsPercetange, PercentageValue, IsFullTime, IsPartTime, IsByHours, IsByService
                FROM dbo.Benefits
                WHERE OfferedBy = @CompanyID AND IsAPI = 0;
            ";

            using var cmd = new SqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CompanyID", companyID);

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
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
