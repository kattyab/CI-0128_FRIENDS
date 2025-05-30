using System.Text;
using System.Text.Json;
using Kaizen.Server.Application.Dtos.ApiDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Infrastructure.Helpers.ApiDeductions;

namespace Kaizen.Server.Infrastructure.Services.ApiDeductions;

public class ExternalApiCaller : IExternalApiCaller
{
    private readonly HttpClient _httpClient = new();

    public async Task<decimal> FetchDeductionAsync(APIsDto config, Dictionary<string, string> runtimeParams)
    {
        string resolvedParams = PlaceholderResolver.Resolve(config.ParametersJson, runtimeParams);
        HttpRequestMessage request;

        if (config.HttpMethod?.ToUpper() == "GET")
        {
            var queryParams = JsonSerializer.Deserialize<Dictionary<string, string>>(resolvedParams);
            string url = PlaceholderResolver.BuildUrlWithQuery(config.Path, queryParams);
            request = new HttpRequestMessage(HttpMethod.Get, url);
        }
        else
        {
            request = new HttpRequestMessage(HttpMethod.Post, config.Path);
            request.Content = new StringContent(resolvedParams, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrWhiteSpace(config.AuthHeaderName))
            request.Headers.Add(config.AuthHeaderName, config.AuthToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        return config.ExpectedDataType switch
        {
            "decimal" => decimal.Parse(body),
            "int" => Convert.ToDecimal(int.Parse(body)),
            var s when s.StartsWith("json-path:") =>
                PlaceholderResolver.ExtractFromJson(body, s.Substring("json-path:$.".Length).Trim()),
            _ => throw new NotSupportedException("Unsupported type")
        };
    }
}
