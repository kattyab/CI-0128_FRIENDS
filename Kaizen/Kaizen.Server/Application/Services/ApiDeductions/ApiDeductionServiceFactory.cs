using Kaizen.Server.Application.Interfaces.ApiDeductions;

namespace Kaizen.Server.Application.Services.ApiDeductions;

public class ApiDeductionServiceFactory : IApiDeductionServiceFactory
{
    private readonly IApiBenefitRepository _repository;
    private readonly IExternalApiCaller _apiCaller;

    public ApiDeductionServiceFactory(
        IApiBenefitRepository repository,
        IExternalApiCaller apiCaller)
    {
        _repository = repository;
        _apiCaller = apiCaller;
    }

    public IApiDeductionService Create(Guid companyId)
    {
        return new ApiDeductionService(companyId, _repository, _apiCaller);
    }
}
