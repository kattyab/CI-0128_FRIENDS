using System.Text;
using System.Text.Json;
using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Infrastructure.Helpers.ApiDeductions;

namespace Kaizen.Server.Infrastructure.Services.ApiDeductions;

public class ExternalApiCaller : IExternalApiCaller
{
    private readonly HttpClient _httpClient = new();

    public async Task<decimal> FetchDeductionAsync(APIsDto apiConfig, Dictionary<string, string> runtimeParameters)
    {
        string resolvedParameters = PlaceholderResolver.Resolve(apiConfig.ParametersJson, runtimeParameters);
        HttpRequestMessage request;

        if (apiConfig.HttpMethod?.ToUpper() == "GET")
        {
            var queryParameters = JsonSerializer.Deserialize<Dictionary<string, string>>(resolvedParameters);
            string url = PlaceholderResolver.BuildUrlWithQuery(apiConfig.Path, queryParameters);
            request = new HttpRequestMessage(HttpMethod.Get, url);
        }
        else
        {
            request = new HttpRequestMessage(HttpMethod.Post, apiConfig.Path);
            request.Content = new StringContent(resolvedParameters, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrWhiteSpace(apiConfig.AuthorizationHeader))
            request.Headers.Add(apiConfig.AuthorizationHeader, apiConfig.AuthorizationToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        return apiConfig.ExpectedDataType switch
        {
            "decimal" => decimal.Parse(body),
            "int" => Convert.ToDecimal(int.Parse(body)),
            var stringType when stringType.StartsWith("json-path:") =>
                PlaceholderResolver.ExtractFromJson(body, stringType.Substring("json-path:$.".Length).Trim()),
            _ => throw new NotSupportedException("Unsupported type")
        };
    }
}
