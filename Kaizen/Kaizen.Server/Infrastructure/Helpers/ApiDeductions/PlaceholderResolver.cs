using System.Text.Json;

namespace Kaizen.Server.Infrastructure.Helpers.ApiDeductions;

public static class PlaceholderResolver
{
    public static string Resolve(string input, Dictionary<string, string> values)
    {
        if (string.IsNullOrEmpty(input)) return input;

        foreach (var kvp in values)
        {
            input = input.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        return input;
    }

    public static string BuildUrlWithQuery(string baseUrl, Dictionary<string, string> parameters)
    {
        var query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
        return $"{baseUrl}?{query}";
    }

    public static decimal ExtractFromJson(string json, string propertyName)
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty(propertyName, out var prop))
            return prop.GetDecimal();

        throw new Exception($"Property '{propertyName}' not found in response.");
    }
}
