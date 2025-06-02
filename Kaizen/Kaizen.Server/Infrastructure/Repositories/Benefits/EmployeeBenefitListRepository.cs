using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Benefits
{
    public class EmployeeBenefitListRepository : IEmployeeBenefitListRepository
    {
        private readonly string _connectionString;

        public EmployeeBenefitListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "The connection string 'KaizenDb' is not defined in appsettings.json");
        }

        public async Task<List<EmployeeBenefitListDto>> GetEmployeeBenefitList(string email)
        
        {
            var benefits = new List<EmployeeBenefitListDto>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    combined.BenefitID,
                    combined.APIId,
                    combined.Name,
                    combined.Type,
                    combined.Value,
                    combined.MinMonths
                FROM (
                    -- Original query for Benefits
                    SELECT 
                        b.Id AS BenefitID,
                        NULL as APIId,
                        b.Name,
                        CASE 
                            WHEN b.IsFixed = 1 THEN 'Fixed'
                            WHEN b.IsPercetange = 1 THEN 'Percentage'
                            ELSE 'Other'
                        END AS Type,
                        CASE 
                            WHEN b.IsFixed = 1 THEN b.FixedValue
                            WHEN b.IsPercetange = 1 THEN b.PercentageValue
                            ELSE 0
                        END AS Value,
                        b.MinWorkDurationMonths AS MinMonths,
                        0 AS IsApi
                    FROM Users u
                    INNER JOIN Employees e ON u.PersonPK = e.PersonPK
                    INNER JOIN ChosenBenefits cb ON e.EmpID = cb.EmployeeID
                    INNER JOIN Benefits b ON cb.BenefitID = b.ID
                    WHERE u.Email = @Email
    
                    UNION ALL
    
                    SELECT 
                        NULL AS BenefitID,
                        adc.ID as APIId,
                        adc.Name,
                        'IsApi' AS Type,
                        0 AS Value,
                        0 AS MinMonths,
                        1 AS IsApi
                    FROM Users u
                    INNER JOIN Employees e ON u.PersonPK = e.PersonPK
                    INNER JOIN ChosenAPIs capi ON e.EmpID = capi.EmployeePK
                    INNER JOIN ApiDeductionConfigs adc ON capi.ApiID = adc.Id
                    WHERE u.Email = @Email
                ) AS combined
                ORDER BY combined.Name;";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                benefits.Add(new EmployeeBenefitListDto
                {
                    BenefitId = reader.IsDBNull("BenefitId") ? (Guid?)null : reader.GetGuid("BenefitId"),
                    APIId = reader.IsDBNull("APIId") ? (int?)null : reader.GetInt32("APIId"),
                    Name = reader.IsDBNull("Name") ? null : reader.GetString("Name"),
                    Type = reader.IsDBNull("Type") ? null : reader.GetString("Type"),
                    Value = reader.IsDBNull("Value") ? 0 : reader.GetDecimal("Value"),
                    MinMonths = reader.IsDBNull("MinMonths") ? 0 : reader.GetInt32("MinMonths")
                });
            }

            return benefits;
        }
    }
}