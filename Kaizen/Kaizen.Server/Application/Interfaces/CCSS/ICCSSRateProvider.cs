using Kaizen.Server.Application.Dtos.CCSS;

namespace Kaizen.Server.Application.Interfaces.CCSS
{
    public interface ICCSSRateProvider
    {
        CCSSRates GetRates();
    }
}