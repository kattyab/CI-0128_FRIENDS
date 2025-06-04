using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Benefits
{
    public class BenefitEmployeeListRepository : IBenefitEmployeeListRepository
    {
        private readonly string _connectionString;

        public BenefitEmployeeListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "The connection string 'KaizenDb' is not defined in appsettings.json");
        }

        public async Task<List<BenefitEmployeeListDto>> GetEmployeeBenefitList(string email)
        
        {
            var benefits = new List<BenefitEmployeeListDto>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                WITH EmployeeCompany AS (
    SELECT 
        e.EmpID,
        e.WorksFor AS CompanyPK,
        c.MaxBenefits
    FROM Users u
    INNER JOIN Employees e ON u.PersonPK = e.PersonPK
    INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
    WHERE u.Email = @Email
)

SELECT 
    b.Id AS BenefitID,
    NULL AS APIId,
    b.Name,
    CASE 
        WHEN b.IsFixed = 1 THEN 'Fixed'
        WHEN b.IsPercentage = 1 THEN 'Percentage'
        ELSE 'Other'
    END AS Type,
    CASE 
        WHEN b.IsFixed = 1 THEN b.FixedValue
        WHEN b.IsPercentage = 1 THEN b.PercentageValue
        ELSE 0
    END AS Value,
    b.MinWorkDurationMonths AS MinMonths,
    ec.MaxBenefits

FROM EmployeeCompany ec
LEFT JOIN ChosenBenefits cb ON cb.EmployeeID = ec.EmpID
LEFT JOIN Benefits b ON cb.BenefitID = b.ID

UNION ALL

SELECT 
    NULL AS BenefitID,
    adc.ID as APIId,
    adc.Name,
    'IsApi' AS Type,
    0 AS Value,
    0 AS MinMonths,
    ec.MaxBenefits

FROM EmployeeCompany ec
LEFT JOIN ChosenAPIs capi ON capi.EmployeePK = ec.EmpID
LEFT JOIN ApiDeductionConfigs adc ON capi.ApiID = adc.Id;";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                benefits.Add(new BenefitEmployeeListDto
                {
                    BenefitId = reader.IsDBNull("BenefitId") ? (Guid?)null : reader.GetGuid("BenefitId"),
                    APIId = reader.IsDBNull("APIId") ? (int?)null : reader.GetInt32("APIId"),
                    Name = reader.IsDBNull("Name") ? null : reader.GetString("Name"),
                    Type = reader.IsDBNull("Type") ? null : reader.GetString("Type"),
                    Value = reader.IsDBNull("Value") ? 0 : reader.GetDecimal("Value"),
                    MinMonths = reader.IsDBNull("MinMonths") ? 0 : reader.GetInt32("MinMonths"),
                    MaxBenefits = reader.IsDBNull("MaxBenefits") ? 0 : reader.GetInt32("MaxBenefits")
                });
            }

            return benefits;
        }
    }
}