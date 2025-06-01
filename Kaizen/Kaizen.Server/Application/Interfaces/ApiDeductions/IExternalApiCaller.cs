using Kaizen.Server.Application.Dtos.ApiDeductions;

namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IExternalApiCaller
{
    Task<decimal> FetchDeductionAsync(APIsDto configuration, Dictionary<string, string> runtimeParameters);
}
