using Kaizen.Server.Application.Dtos.CCSS;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Infrastructure.Helpers.CCSS;

namespace Kaizen.Server.Infrastructure.Services.CCSS
{
    public class CCSSRateFileProvider : ICCSSRateProvider
    {
        private readonly IConfiguration _config;

        public CCSSRateFileProvider(IConfiguration config)
        {
            _config = config;
        }

        public CCSSRates GetRates()
        {
            var configPath = _config["Paths:CCSSRatesFile"];
            return CCSSRateLoader.LoadFromFile(configPath!);
        }
    }
}