using Kaizen.Server.Application.Dtos.ApiDeductions;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IExternalApiCaller
{
    Task<decimal> FetchDeductionAsync(APIsDto config, Dictionary<string, string> runtimeParams);
}
