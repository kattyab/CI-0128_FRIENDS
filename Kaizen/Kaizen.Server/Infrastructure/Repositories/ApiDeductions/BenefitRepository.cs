using System.Data;
using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories.ApiDeductions;

public class BenefitRepository : IApiBenefitRepository
{
    private readonly SqlConnection _connection;

    public BenefitRepository(SqlConnection connection)
    {
        _connection = connection;
    }


    public async Task<List<APIsDto>> GetBenefitsAsync(Guid companyId)
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        var benefits = new List<APIsDto>();
        const string query = @"SELECT 
            adc.Id, 
            adc.Name, 
            adc.Endpoint AS Path, 
            adc.HttpMethod, 
            adc.AuthHeaderName,
            adc.AuthToken, 
            adc.ParametersJson, 
            adc.ExpectedDataType
        FROM dbo.Companies c
        INNER JOIN dbo.OffersAPIs oa ON oa.CompanyPK = c.CompanyPK
        INNER JOIN dbo.ApiDeductionConfigs adc ON adc.Id = oa.ApiConfigId
        WHERE c.CompanyPK = @CompanyId;";

        using var command = new SqlCommand(query, _connection);
        command.Parameters.Add("@CompanyId", SqlDbType.UniqueIdentifier).Value = companyId;
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            benefits.Add(new APIsDto
            {
                ID = reader.GetInt32(0),
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
        const string query = @"SELECT 
            eap.EmployeeId, 
            adc.Id, 
            eap.ParameterKey, 
            eap.ParameterValue
        FROM dbo.EmployeeApiParameters eap
        INNER JOIN dbo.ApiDeductionConfigs adc ON eap.ApiConfigId = adc.Id
        INNER JOIN dbo.ChosenAPIs ca ON adc.Id = ca.ApiID AND eap.EmployeeId = ca.EmployeePK
        WHERE eap.EmployeeId = @EmployeeId;";
        using var command = new SqlCommand(query, _connection);
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            parameters.Add(new EmployeeBenefitParameterDto
            {
                EmployeeId = reader.GetGuid(0),
                BenefitId = reader.GetInt32(1),
                Key = reader.GetString(2),
                Value = reader.GetString(3)
            });
        }
        return parameters;
    }
}
