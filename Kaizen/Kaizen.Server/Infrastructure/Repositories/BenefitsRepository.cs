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
                    0 AS ApiID,
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
                OfferedBy = @OfferedBy

                UNION ALL

                SELECT
                    NULL AS ID,
                Id AS ApiID,
                    Name,
                    0 AS MinWorkDurationMonths,
                    CAST(0 AS BIT) AS IsFixed,
                    NULL AS FixedValue,
                    CAST(0 AS BIT) AS IsPercentage,
                    NULL AS PercentageValue,
                    CAST(1 AS BIT) AS IsFullTime,
                    CAST(1 AS BIT) AS IsPartTime,
                    CAST(1 AS BIT) AS IsByHours,
                    CAST(1 AS BIT) AS IsByService
                FROM
                    ApiDeductionConfigs
                INNER JOIN
                    OffersAPIs oa ON ApiDeductionConfigs.Id = oa.ApiConfigId
                WHERE
	                CompanyPK = @OfferedBy
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
                    ApiID = reader.GetInt32(reader.GetOrdinal("ApiID")),
                    ID = reader.IsDBNull(reader.GetOrdinal("ID"))
                        ? (Guid?)null
                        : reader.GetGuid(reader.GetOrdinal("ID")),
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

        public void DeleteBenefit(Guid guid, SqlConnection connection, SqlTransaction transaction)
        {
            const string deleteBenefitCommandText = @"
        DELETE Benefits
        WHERE ID = @ID;";

            using var command = new SqlCommand(deleteBenefitCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@ID", guid));
            command.ExecuteNonQuery();
        }

        public void DeleteBenefit(int id, Guid companyPK, SqlConnection connection, SqlTransaction transaction)
        {
            const string deleteBenefitCommandText = @"
        INSERT INTO Notifications (Description, NotificationDate, UserPK)
        SELECT
            CONCAT('La deducción de API ', ISNULL(adc.Name, 'seleccionada'), ' ya no está disponible para su empresa.'),
            GETDATE(),
            u.UserPK
        FROM OffersAPIs oa
        INNER JOIN ApiDeductionConfigs adc ON oa.ApiConfigId = adc.ID
        INNER JOIN ChosenAPIs ca ON ca.ApiID = oa.ApiConfigId
        INNER JOIN Employees e ON ca.EmployeePK = e.EmpId AND e.WorksFor = oa.CompanyPK
        INNER JOIN Persons p ON e.PersonPK = p.PersonPK
        INNER JOIN Users u ON p.PersonPK = u.PersonPK
        WHERE oa.CompanyPK = @CompanyPK AND oa.ApiConfigId = @ID;
        
        -- Delete from ChosenAPIs for employees of Company
        DELETE ca
        FROM ChosenAPIs ca
        INNER JOIN OffersAPIs oa ON ca.ApiID = oa.ApiConfigId
        INNER JOIN Employees e ON ca.EmployeePK = e.EmpId AND e.WorksFor = oa.CompanyPK
        WHERE oa.CompanyPK = @CompanyPK AND oa.ApiConfigId = @ID;";

            using var command = new SqlCommand(deleteBenefitCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@ID", id));
            command.Parameters.Add(new SqlParameter("@CompanyPK", companyPK));
            command.ExecuteNonQuery();
        }

        public void SoftDeleteAndNotifyBenefit(Guid benefitId, SqlConnection connection, SqlTransaction transaction)
        {
            const string softDeleteCommandText = @"
        UPDATE Benefits
        SET IsOut = 1
        WHERE ID = @BenefitID;

        DECLARE @BenefitName NVARCHAR(100);
        SELECT @BenefitName = Name FROM Benefits WHERE ID = @BenefitID; 

        INSERT INTO Notifications (Description, NotificationDate, UserPK)
        SELECT
            'El beneficio ' + @BenefitName + ' ha dejado de ofrecerse. Podrá seguir disfrutándolo hasta el fin de este mes.',
            GETDATE(),
            u.UserPK
        FROM ChosenBenefits cb
        INNER JOIN Employees e ON cb.EmployeeID = e.EmpId
        INNER JOIN Persons p ON e.PersonPK = p.PersonPK
        INNER JOIN Users u ON p.PersonPK = u.PersonPK
        WHERE cb.BenefitID = @BenefitID";

            using var command = new SqlCommand(softDeleteCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@BenefitID", benefitId));
            command.ExecuteNonQuery();
        }

        public void FullDeleteAndNotifyBenefit(Guid benefitId, SqlConnection connection, SqlTransaction transaction)
        {
            const string fullDeleteCommandText = @"
        DECLARE @BenefitName NVARCHAR(100);
        SELECT @BenefitName = Name FROM Benefits WHERE ID = @BenefitID;

        INSERT INTO Notifications (Description, NotificationDate, UserPK)
        SELECT
            'El beneficio ' + @BenefitName + ' ha dejado de ofrecerse.',
            GETDATE(),
            u.UserPK
        FROM ChosenBenefits cb
        INNER JOIN Employees e ON cb.EmployeeID = e.EmpId
        INNER JOIN Persons p ON e.PersonPK = p.PersonPK
        INNER JOIN Users u ON p.PersonPK = u.PersonPK
        WHERE cb.BenefitID = @BenefitID

        DELETE FROM ChosenBenefits WHERE BenefitID = @BenefitID
        DELETE FROM Benefits WHERE ID = @BenefitID";

            using var command = new SqlCommand(fullDeleteCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@BenefitID", benefitId));
            command.ExecuteNonQuery();
        }

        public bool GetIfBenefitIsSubscribed(Guid? benefitID, SqlConnection connection, SqlTransaction transaction)
        {
            const string checkSubscriptionCommandText = @"
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM ChosenBenefits WHERE BenefitID = @BenefitID
        ) THEN 1 ELSE 0
        END;";

            using var command = new SqlCommand(checkSubscriptionCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@BenefitID", benefitID));

            var result = command.ExecuteScalar();
            return result is int intResult && intResult == 1;
        }

        private bool GetIfBenefitIsSubscribed(Guid? benefitID)
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

        public bool GetIfBenefitIsOnPayroll(Guid? benefitID, SqlConnection connection, SqlTransaction transaction)
        {
            const string checkPayrollCommandText = @"
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM OptionalDeductions WHERE BenefitID = @BenefitID
        ) THEN 1 ELSE 0
        END;";

            using var command = new SqlCommand(checkPayrollCommandText, connection, transaction);
            command.Parameters.Add(new SqlParameter("@BenefitID", benefitID));

            var result = command.ExecuteScalar();
            return result is int intResult && intResult == 1;
        }
    }
}
