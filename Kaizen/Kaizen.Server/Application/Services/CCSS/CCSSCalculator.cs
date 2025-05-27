using Kaizen.Server.Application.Interfaces.CCSS;

namespace Kaizen.Server.Application.Services.CCSS
{
    public class CCSSCalculator : ICCSSCalculator
    {
        private readonly ICCSSRateProvider _rateProvider;

        private const int SecondDecimal = 2;

        public CCSSCalculator(ICCSSRateProvider rateProvider)
        {
            _rateProvider = rateProvider;
        }

        public decimal CalculateDeduction(decimal grossSalary)
        {
            var rates = _rateProvider.GetRates();
            var totalRate = rates.SEM + rates.IVM + rates.LPT;
            var deduction = grossSalary * totalRate;
            return Math.Round(deduction, SecondDecimal, MidpointRounding.ToEven);
        }
    }
}