using Kaizen.Server.Application.Dtos.BenefitDeductions;
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
                    NULL AS ApiID,
                    Name,
                    MinWorkDurationMonths,
                    IsFixed,
                    FixedValue,
                    IsPercentage,
                    PercentageValue,
                    0 AS IsAPI,
                    IsFullTime,
                    IsPartTime,
                    IsByHours,
                    IsByService,
                    NULL AS Endpoint
                FROM
                    Benefits
                WHERE
                    OfferedBy = @OfferedBy

                UNION ALL

                SELECT
                    NULL AS ID,
                    adc.Id AS ApiID,
                    adc.Name,
                    0 AS MinWorkDurationMonths,
                    0 AS IsFixed,
                    NULL AS FixedValue,
                    0 AS IsPercentage,
                    NULL AS PercentageValue,
                    1 AS IsAPI,
                    1 AS IsFullTime,
                    1 AS IsPartTime,
                    1 AS IsByHours,
                    1 AS IsByService,
                    adc.Endpoint
               FROM
                    ApiDeductionConfigs adc
               JOIN
                    OffersAPIs oa ON adc.Id = oa.ApiConfigId
               WHERE
                    oa.CompanyPK = @OfferedBy
";

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
                    ID = reader.IsDBNull(reader.GetOrdinal("ID"))
                        ? null
                        : reader.GetGuid(reader.GetOrdinal("ID")),
                    ApiID = reader.IsDBNull(reader.GetOrdinal("ApiID"))
                        ? null
                        : (int?)reader.GetInt32(reader.GetOrdinal("ApiID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    MinWorkDurationMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths")),
                    IsFixed = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFixed"))),
                    FixedValue = reader.IsDBNull(reader.GetOrdinal("FixedValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("FixedValue")),
                    IsPercentage = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPercentage"))),
                    PercentageValue = reader.IsDBNull(reader.GetOrdinal("PercentageValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("PercentageValue")),
                    IsAPI = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsAPI"))),
                    IsFullTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFullTime"))),
                    IsPartTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPartTime"))),
                    IsByHours = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByHours"))),
                    IsByService = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByService"))),
                    Endpoint = reader.IsDBNull(reader.GetOrdinal("Endpoint"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Endpoint"))
                };

                benefits.Add(benefit);
            }
            return benefits;
        }

        // GetBenefit for GUID
        public BenefitDto? GetBenefit(Guid guid, Guid companyPK)
        {
            BenefitDto? benefit = null;

            const string getBenefitCommandText = @"
                SELECT
                TOP 1
                    ID,
                    NULL AS ApiID,
                    Name,
                    MinWorkDurationMonths,
                    IsFixed,
                    FixedValue,
                    IsPercentage,
                    PercentageValue,
                    0 AS IsAPI,
                    IsFullTime,
                    IsPartTime,
                    IsByHours,
                    IsByService,
                    NULL AS Endpoint,
                    0 AS IsAPIActive
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
                    ID = reader.IsDBNull(reader.GetOrdinal("ID"))
                        ? null
                        : reader.GetGuid(reader.GetOrdinal("ID")),
                    ApiID = reader.IsDBNull(reader.GetOrdinal("ApiID"))
                        ? null
                        : (int?)reader.GetInt32(reader.GetOrdinal("ApiID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    MinWorkDurationMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths")),
                    IsFixed = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFixed"))),
                    FixedValue = reader.IsDBNull(reader.GetOrdinal("FixedValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("FixedValue")),
                    IsPercentage = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPercentage"))),
                    PercentageValue = reader.IsDBNull(reader.GetOrdinal("PercentageValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("PercentageValue")),
                    IsAPI = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsAPI"))),
                    IsFullTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFullTime"))),
                    IsPartTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPartTime"))),
                    IsByHours = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByHours"))),
                    IsByService = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByService"))),
                    Endpoint = reader.IsDBNull(reader.GetOrdinal("Endpoint"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Endpoint")),
                    IsAPIActive = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsAPIActive")))
                };

                benefit.IsSubscribed = this.GetIfBenefitIsSubscribed(benefit.ID);
            }

            return benefit;
        }

        // GetBenefit for INT
        public BenefitDto? GetBenefit(int id, Guid companyPK)
        {
            BenefitDto? benefit = null;

            const string getBenefitCommandText = @"
                SELECT
                TOP 1
                    NULL AS ID,
                    adc.Id AS ApiID,
                    Name,
                    0 AS MinWorkDurationMonths,
                    0 AS IsFixed,
                    NULL AS FixedValue,
                    0 AS IsPercentage,
                    NULL AS PercentageValue,
                    1 AS IsAPI,
                    1 AS IsFullTime,
                    1 AS IsPartTime,
                    1 AS IsByHours,
                    1 AS IsByService,
                    adc.Endpoint,
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM OffersAPIs oa 
                            WHERE oa.CompanyPK = @OfferedBy
                                AND oa.ApiConfigId = @ID
                        ) THEN 1 
                        ELSE 0 
                    END AS IsAPIActive
                FROM
                    ApiDeductionConfigs adc
                WHERE
                    adc.Id = @ID";

            SqlParameter[] getBenefitParameters = [
                new SqlParameter("@ID", id),
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
                    ID = reader.IsDBNull(reader.GetOrdinal("ID"))
                        ? null
                        : reader.GetGuid(reader.GetOrdinal("ID")),
                    ApiID = reader.IsDBNull(reader.GetOrdinal("ApiID"))
                        ? null
                        : (int?)reader.GetInt32(reader.GetOrdinal("ApiID")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    MinWorkDurationMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths")),
                    IsFixed = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFixed"))),
                    FixedValue = reader.IsDBNull(reader.GetOrdinal("FixedValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("FixedValue")),
                    IsPercentage = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPercentage"))),
                    PercentageValue = reader.IsDBNull(reader.GetOrdinal("PercentageValue"))
                        ? null
                        : (decimal?)reader.GetDecimal(reader.GetOrdinal("PercentageValue")),
                    IsAPI = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsAPI"))),
                    IsFullTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsFullTime"))),
                    IsPartTime = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsPartTime"))),
                    IsByHours = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByHours"))),
                    IsByService = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsByService"))),
                    Endpoint = reader.IsDBNull(reader.GetOrdinal("Endpoint"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Endpoint")),
                    IsAPIActive = Convert.ToBoolean(reader.GetValue(reader.GetOrdinal("IsAPIActive")))
                };
                benefit.IsSubscribed = true; // Disables edit.
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
            IF (SELECT IsDeleted FROM Companies WHERE CompanyPK = @OfferedBy) = 0
            BEGIN
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
                    OfferedBy = @OfferedBy
            END;";

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

        private bool GetIfBenefitIsSubscribed(Guid? benefitID)
        {
            if (benefitID == null) { return false; }
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
