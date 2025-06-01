using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using Microsoft.Data.SqlClient;

public class OfferedBenefitsRepository : IOfferedBenefitsRepository
{
    private readonly string _connectionString;

    public OfferedBenefitsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException("Connection string 'KaizenDb' not found.");
    }

    public async Task<List<OfferedBenefitDto>> GetAvailableBenefitsForEmployee(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // 1. Get employee's contract type and months at company
        var employeeQuery = @"
            SELECT 
                e.ContractType,
                DATEDIFF(MONTH, e.StartDate, GETDATE()) AS MonthsInCompany,
                c.CompanyPK
            FROM Users u
            INNER JOIN Persons p ON u.PersonPK = p.PersonPK
            INNER JOIN Employees e ON e.PersonPK = p.PersonPK
            INNER JOIN Companies c ON e.WorksFor = c.CompanyPK
            WHERE u.Email = @Email";

        string contractType = string.Empty;
        int monthsInCompany = 0;
        Guid companyPk = Guid.Empty;

        using (var cmd = new SqlCommand(employeeQuery, connection))
        {
            cmd.Parameters.AddWithValue("@Email", email);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                throw new KeyNotFoundException($"No employee found for email {email}");

            contractType = reader.GetString(reader.GetOrdinal("ContractType"));
            monthsInCompany = reader.GetInt32(reader.GetOrdinal("MonthsInCompany"));
            companyPk = reader.GetGuid(reader.GetOrdinal("CompanyPK"));
        }

        // 2. Get all benefits for the employee's company
        var benefitQuery = @"
        SELECT 
            b.Name,
            CASE 
                WHEN b.IsFixed = 1 THEN 'Fixed'
                WHEN b.IsPercetange = 1 THEN 'Percentage'
                ELSE 'Other'
            END AS Type,
            b.MinWorkDurationMonths,
            b.IsPartTime,
            b.IsFullTime,
            b.IsByHours,
            b.IsByService,
            CASE 
                WHEN b.IsFixed = 1 THEN b.FixedValue
                WHEN b.IsPercetange = 1 THEN b.PercentageValue
                ELSE 0
            END AS Value
        FROM Benefits b
        WHERE b.OfferedBy = @CompanyPK

        UNION ALL

        SELECT 
            adc.Name,
            'IsApi' AS Type,
            0 AS MinWorkDurationMonths,
            1, 1, 1, 1, -- All contract types
            0 AS Value
        FROM ApiDeductionConfigs adc
        INNER JOIN OffersAPIs oa ON adc.Id = oa.ApiConfigId
        WHERE oa.CompanyPK = @CompanyPK";

        var benefits = new List<OfferedBenefitDto>();

        using (var cmd = new SqlCommand(benefitQuery, connection))
        {
            cmd.Parameters.AddWithValue("@CompanyPK", companyPk);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var minMonths = reader.GetInt32(reader.GetOrdinal("MinWorkDurationMonths"));

                var isEligible = IsContractTypeEligible(
                    contractType,
                    isPartTime: reader.GetInt32(reader.GetOrdinal("IsPartTime")) == 1,
                    isFullTime: reader.GetInt32(reader.GetOrdinal("IsFullTime")) == 1,
                    isByHours: reader.GetInt32(reader.GetOrdinal("IsByHours")) == 1,
                    isByService: reader.GetInt32(reader.GetOrdinal("IsByService")) == 1
                );

                var isTenureEligible = monthsInCompany >= minMonths;
                var available = isEligible && isTenureEligible;

                var reason = string.Empty;
                if (!available)
                {
                    var reasons = new List<string>();
                    if (!isEligible) reasons.Add("Contract type not eligible");
                    if (!isTenureEligible) reasons.Add($"Requires {minMonths} months (has {monthsInCompany})");
                    reason = string.Join(", ", reasons);
                }

                benefits.Add(new OfferedBenefitDto
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    MinMonths = minMonths,
                    IsAvailable = available,
                    ReasonUnavailable = available ? null : reason,
                    Value = reader.GetDecimal(reader.GetOrdinal("Value")) // <-- Added
                });
            }
        }

        return benefits;
    }

    private bool IsContractTypeEligible(string contractType, bool isPartTime, bool isFullTime, bool isByHours, bool isByService)
    {
        return contractType switch
        {
            "Medio Tiempo" => isPartTime,             // Part-Time
            "Tiempo Completo" => isFullTime,          // Full-Time
            "Por Horas" => isByHours,                 // By Hours
            "Servicios Profesionales" => isByService, // By Service
            _ => false
        };
    }

}
