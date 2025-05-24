using System.Text.Json;

namespace Kaizen.Server.Infrastructure.Helpers.IncomeTax
{
    public class IncomeTaxBracket
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Rate { get; set; }
    }

    public static class IncomeTaxBracketLoader
    {
        public static List<IncomeTaxBracket> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Income tax brackets file not found", filePath);

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<IncomeTaxBracket>>(json)
                ?? throw new InvalidOperationException("Invalid tax bracket configuration.");
        }
    }
}
