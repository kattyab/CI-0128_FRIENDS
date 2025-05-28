using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;

namespace Kaizen.Server.Application.Services.ApiDeductions;

public class DeductionService : IDeductionService
{
    private readonly Guid _companyId;
    private readonly IBenefitRepository _repository;
    private readonly IExternalApiCaller _apiCaller;

    public DeductionService(Guid companyId, IBenefitRepository repository, IExternalApiCaller apiCaller)
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
                result[benefit.Name] = -1;
                continue;
            }

            try
            {
                var deduction = await _apiCaller.FetchDeductionAsync(benefit, paramDict);
                result[benefit.Name] = deduction;
            }
            catch
            {
                result[benefit.Name] = -1;
            }
        }

        return result;
    }
}
