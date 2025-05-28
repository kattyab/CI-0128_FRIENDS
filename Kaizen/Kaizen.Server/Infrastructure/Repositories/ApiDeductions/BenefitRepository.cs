using System.Data;
using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories.ApiDeductions;

public class BenefitRepository : IBenefitRepository
{
    private readonly SqlConnection _connection;

    public BenefitRepository(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<BenefitDto>> GetBenefitsAsync(Guid companyId)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        var benefits = new List<BenefitDto>();

        const string query = @"SELECT b.ID, b.Name, b.Path, adc.HttpMethod, adc.AuthHeaderName,
                                      adc.AuthToken, adc.ParametersJson, adc.ExpectedDataType
                               FROM dbo.Companies c
                               INNER JOIN dbo.Benefits b ON b.OfferedBy = c.CompanyPK
                               LEFT JOIN dbo.ApiDeductionConfigs adc ON b.ID = adc.BenefitsID
                               WHERE c.CompanyPK = @CompanyId AND b.IsAPI = 1 AND adc.Id IS NOT NULL;";

        using var cmd = new SqlCommand(query, _connection);
        cmd.Parameters.Add("@CompanyId", SqlDbType.UniqueIdentifier).Value = companyId;

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            benefits.Add(new BenefitDto
            {
                ID = reader.GetGuid(0),
                Name = reader.GetString(1),
                Path = reader.IsDBNull(2) ? null : reader.GetString(2),
                HttpMethod = reader.IsDBNull(3) ? null : reader.GetString(3),
                AuthHeaderName = reader.IsDBNull(4) ? null : reader.GetString(4),
                AuthToken = reader.IsDBNull(5) ? null : reader.GetString(5),
                ParametersJson = reader.IsDBNull(6) ? null : reader.GetString(6),
                ExpectedDataType = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }

        return benefits;
    }

    public async Task<List<EmployeeBenefitParameterDto>> GetParametersForEmployeeAsync(Guid employeeId)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        var parameters = new List<EmployeeBenefitParameterDto>();

        const string query = @"SELECT eap.EmployeeId, b.ID, eap.ParameterKey, eap.ParameterValue
                               FROM dbo.EmployeeApiParameters eap
                               INNER JOIN dbo.ApiDeductionConfigs adc ON eap.ApiConfigId = adc.Id
                               INNER JOIN dbo.Benefits b ON adc.BenefitsID = b.ID
                               INNER JOIN dbo.ChosenBenefits cb ON b.ID = cb.BenefitID AND eap.EmployeeId = cb.EmployeeID
                               WHERE eap.EmployeeId = @EmployeeId";

        using var cmd = new SqlCommand(query, _connection);
        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            parameters.Add(new EmployeeBenefitParameterDto
            {
                EmployeeId = reader.GetGuid(0),
                BenefitId = reader.GetGuid(1),
                Key = reader.GetString(2),
                Value = reader.GetString(3)
            });
        }

        return parameters;
    }
}
