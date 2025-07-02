using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Infrastructure.Contexts;

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

    public async Task<Dictionary<string, decimal>> GetDeductionsForEmployeeAsync(Guid employeeId, PayrollTransactionContext context = null)
    {
        var benefits = await _repository.GetBenefitsAsync(_companyId, context);
        var allParameters = await _repository.GetParametersForCompanyAsync(_companyId, context);
        var employeeParameters = allParameters.Where(parameter => parameter.EmployeeId == employeeId).ToList();

        var parametersByBenefitId = employeeParameters
            .GroupBy(parameter => parameter.BenefitId)
            .ToDictionary(
                benefitGroup => benefitGroup.Key,
                benefitGroup => benefitGroup.ToDictionary(parameter => parameter.Key, parameter => parameter.Value)
            );

        var result = new Dictionary<string, decimal>();
        foreach (var benefit in benefits)
        {
            if (!parametersByBenefitId.TryGetValue(benefit.ID, out var parameterDictionary))
            {
                continue;
            }
            try
            {
                var deduction = await _apiCaller.FetchDeductionAsync(benefit, parameterDictionary);
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
