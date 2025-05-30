using Kaizen.Server.Application.Interfaces.ApiDeductions;

namespace Kaizen.Server.Application.Services.ApiDeductions;

public class ApiDeductionService : IApiDeductionService
{
    private readonly Guid _companyId;
    private readonly IApiBenefitRepository _repository;
    private readonly IExternalApiCaller _apiCaller;

    public ApiDeductionService(Guid companyId, IApiBenefitRepository repository, IExternalApiCaller apiCaller)
    {
        _companyId = companyId;
        _repository = repository;
        _apiCaller = apiCaller;
    }

    public async Task<Dictionary<string, decimal>> GetDeductionsForEmployeeAsync(Guid employeeId)
    {
        var benefits = await _repository.GetBenefitsAsync(_companyId);
        var parameters = await _repository.GetParametersForEmployeeAsync(employeeId);

        var groupedParams = parameters
            .GroupBy(p => p.BenefitId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(p => p.Key, p => p.Value)
            );

        var result = new Dictionary<string, decimal>();

        foreach (var benefit in benefits)
        {
            if (!groupedParams.TryGetValue(benefit.ID, out var paramDict))
            {
                continue;
            }

            try
            {
                var deduction = await _apiCaller.FetchDeductionAsync(benefit, paramDict);
                result[benefit.Name] = deduction;
            }
            catch
            {
                continue;
            }
        }

        return result;
    }
}
