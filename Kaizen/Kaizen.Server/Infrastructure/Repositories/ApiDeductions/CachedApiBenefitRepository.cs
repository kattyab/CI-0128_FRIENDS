using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Infrastructure.Repositories.ApiDeductions;
using Microsoft.Extensions.Caching.Memory;

public class CachedApiBenefitRepository : IApiBenefitRepository
{
    private readonly ApiBenefitDeductionRepository _apiBenefitRepository;
    private readonly IMemoryCache _memoryCache;

    public CachedApiBenefitRepository(ApiBenefitDeductionRepository inner, IMemoryCache cache)
    {
        _apiBenefitRepository = inner;
        _memoryCache = cache;
    }

    public Task<List<APIsDto>> GetBenefitsAsync(Guid companyId)
    {
        string cacheKey = $"benefits-{companyId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
            return _apiBenefitRepository.GetBenefitsAsync(companyId);
        });
    }

    public Task<List<EmployeeBenefitParameterDto>> GetParametersForCompanyAsync(Guid companyId)
    {
        string cacheKey = $"employee-params-{companyId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
            return _apiBenefitRepository.GetParametersForCompanyAsync(companyId);
        });
    }
}
