using System.Text.Json;
using Kaizen.Server.Application.Dtos.CCSS;

namespace Kaizen.Server.Infrastructure.Helpers.CCSS
{
    public static class CCSSRateLoader
    {
        public static CCSSRates LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<CCSSRates>(json)!;
        }
    }
}