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
                    combined.Name,
                    combined.Type,
                    combined.Value,
                    combined.MinMonths
                FROM (
                    -- Original query for Benefits
                    SELECT 
                        b.Name,
                        CASE 
                            WHEN b.IsFixed = 1 THEN 'Fixed'
                            WHEN b.IsPercetange = 1 THEN 'Percentage'
                            ELSE 'Other'
                        END AS Type,
                        CASE 
                            WHEN b.IsFixed = 1 THEN CAST(b.FixedValue AS NVARCHAR(50))
                            WHEN b.IsPercetange = 1 THEN CAST(b.PercentageValue AS NVARCHAR(50)) + '%'
                            ELSE 'N/A'
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
                        adc.Name,
                        'IsApi' AS Type,
                        'API' AS Value,
                        0 AS MinMonths,
                        1 AS IsApi -- Flag to identify source
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
                    Name = reader.GetString("Name"),
                    Type = reader.GetString("Type"),
                    Value = reader.GetString("Value"),
                    MinMonths = reader.GetInt32("MinMonths")
                });
            }

            return benefits;
        }
    }
}