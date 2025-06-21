using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories
{
    public class BenefitsRepository : IBenefitsRepository
    {
        private readonly string _connectionString;

        public BenefitsRepository(IConfiguration configuration)
        {
            this._connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public List<BenefitDto> GetBenefits(Guid companyPK)
        {
            const string getBenefitsCommandText = @"
                SELECT
                    ID,
                    Name,
                    MinWorkDurationMonths,
                    IsFixed,
                    FixedValue,
                    IsPercentage,
                    PercentageValue,
                    IsFullTime,
                    IsPartTime,
                    IsByHours,
                    IsByService
                FROM
                    Benefits
                WHERE
                    OfferedBy = @OfferedBy";

            SqlParameter[] getBenefitsParameters = [
                new SqlParameter("@OfferedBy", companyPK)
            ];

            using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString,
                getBenefitsCommandText,
                CommandType.Text,
                getBenefitsParameters);

            List<BenefitDto> benefits = [];
            while (reader.Read())
            {
                BenefitDto benefit = new()
                {
                    ID = reader.GetGuid(reader.GetOrdinal("ID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    MinWorkDurationMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths")),
                    IsFixed = reader.GetBoolean(reader.GetOrdinal("IsFixed")),
                    FixedValue = reader.IsDBNull(reader.GetOrdinal("FixedValue"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("FixedValue")),
                    IsPercentage = reader.GetBoolean(reader.GetOrdinal("IsPercentage")),
                    PercentageValue = reader.IsDBNull(reader.GetOrdinal("PercentageValue"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("PercentageValue")),
                    IsFullTime = reader.GetBoolean(reader.GetOrdinal("IsFullTime")),
                    IsPartTime = reader.GetBoolean(reader.GetOrdinal("IsPartTime")),
                    IsByHours = reader.GetBoolean(reader.GetOrdinal("IsByHours")),
                    IsByService = reader.GetBoolean(reader.GetOrdinal("IsByService")),
                };

                benefits.Add(benefit);
            }
            return benefits;
        }

        public BenefitDto? GetBenefit(Guid guid, Guid companyPK)
        {
            BenefitDto? benefit = null;

            const string getBenefitCommandText = @"
                SELECT
                TOP 1
                    ID,
                    Name,
                    MinWorkDurationMonths,
                    IsFixed,
                    FixedValue,
                    IsPercentage,
                    PercentageValue,
                    IsFullTime,
                    IsPartTime,
                    IsByHours,
                    IsByService
                FROM
                    Benefits
                WHERE
                    ID = @ID AND
                    OfferedBy = @OfferedBy";

            SqlParameter[] getBenefitParameters = [
                new SqlParameter("@ID", guid),
                new SqlParameter("@OfferedBy", companyPK)
            ];

            using SqlDataReader reader = SqlHelper.ExecuteReader(this._connectionString,
                getBenefitCommandText,
                CommandType.Text,
                getBenefitParameters);

            if (reader.Read())
            {
                benefit = new()
                {
                    ID = reader.GetGuid(reader.GetOrdinal("ID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    MinWorkDurationMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths")),
                    IsFixed = reader.GetBoolean(reader.GetOrdinal("IsFixed")),
                    FixedValue = reader.IsDBNull(reader.GetOrdinal("FixedValue"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("FixedValue")),
                    IsPercentage = reader.GetBoolean(reader.GetOrdinal("IsPercentage")),
                    PercentageValue = reader.IsDBNull(reader.GetOrdinal("PercentageValue"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("PercentageValue")),
                    IsFullTime = reader.GetBoolean(reader.GetOrdinal("IsFullTime")),
                    IsPartTime = reader.GetBoolean(reader.GetOrdinal("IsPartTime")),
                    IsByHours = reader.GetBoolean(reader.GetOrdinal("IsByHours")),
                    IsByService = reader.GetBoolean(reader.GetOrdinal("IsByService")),
                };

                benefit.IsSubscribed = this.GetIfBenefitIsSubscribed(benefit.ID);
            }

            return benefit;
        }


        public void UpdateBenefit(BenefitDto benefit, Guid companyPK)
        {
            if (this.GetIfBenefitIsSubscribed(benefit.ID))
            {
                throw new InvalidOperationException("Cannot update a benefit that is already subscribed to by employees.");
            }

            const string updateBenefitCommandText = @"
                UPDATE
                    Benefits
                SET
                    Name = @Name,
                    MinWorkDurationMonths = @MinWorkDurationMonths,
                    IsFixed = @IsFixed,
                    FixedValue = @FixedValue,
                    IsPercentage = @IsPercentage,
                    PercentageValue = @PercentageValue,
                    IsFullTime = @IsFullTime,
                    IsPartTime = @IsPartTime,
                    IsByHours = @IsByHours,
                    IsByService = @IsByService
                WHERE
                    ID = @ID AND
                    OfferedBy = @OfferedBy;";

            SqlParameter[] updateBenefitParameters = [
                new SqlParameter("@ID", benefit.ID),
                new SqlParameter("@OfferedBy", companyPK),

                new SqlParameter("@Name", benefit.Name),
                new SqlParameter("@MinWorkDurationMonths", benefit.MinWorkDurationMonths),
                new SqlParameter("@IsFixed", benefit.IsFixed),
                new SqlParameter("@FixedValue", (object?)benefit.FixedValue ?? DBNull.Value),
                new SqlParameter("@IsPercentage", benefit.IsPercentage),
                new SqlParameter("@PercentageValue", (object?)benefit.PercentageValue ?? DBNull.Value),
                new SqlParameter("@IsFullTime", benefit.IsFullTime),
                new SqlParameter("@IsPartTime", benefit.IsPartTime),
                new SqlParameter("@IsByHours", benefit.IsByHours),
                new SqlParameter("@IsByService", benefit.IsByService),
            ];

            SqlHelper.ExecuteNonQuery(this._connectionString,
                updateBenefitCommandText,
                CommandType.Text,
                updateBenefitParameters);
        }

        private bool GetIfBenefitIsSubscribed(Guid benefitID)
        {
            const string checkSubscriptionCommandText = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM ChosenBenefits WHERE BenefitID = @BenefitID
                ) THEN 1 ELSE 0
                END;";
            SqlParameter[] checkSubscriptionParameters = [
                new SqlParameter("@BenefitID", benefitID)
            ];
            object? isSubscribedResult = SqlHelper.ExecuteScalar(this._connectionString,
                checkSubscriptionCommandText,
                CommandType.Text,
                checkSubscriptionParameters);
            return isSubscribedResult is int && (int)isSubscribedResult == 1;
        }
    }
}
