using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Infrastructure.Helpers.IncomeTax;

namespace Kaizen.Server.Infrastructure.Services.IncomeTax
{
    public class IncomeTaxBracketFileProvider : IIncomeTaxBracketProvider
    {
        private readonly IConfiguration _config;

        public IncomeTaxBracketFileProvider(IConfiguration config)
        {
            _config = config;
        }

        public List<IncomeTaxBracket> GetBrackets()
        {
            var configPath = _config["Paths:IncomeTaxBracketsFile"];
            return IncomeTaxBracketLoader.LoadFromFile(configPath!);
        }
    }
}
