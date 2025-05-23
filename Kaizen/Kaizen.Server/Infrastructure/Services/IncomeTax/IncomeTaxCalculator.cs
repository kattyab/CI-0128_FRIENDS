using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Infrastructure.Helpers.IncomeTax;

namespace Kaizen.Server.Infrastructure.Services.IncomeTax
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly List<IncomeTaxBracket> _brackets;

        public IncomeTaxCalculator(IIncomeTaxBracketProvider provider)
        {
            _brackets = provider.GetBrackets();
        }

        public decimal Calculate(decimal grossSalary)
        {
            decimal tax = 0;
            foreach (var bracket in _brackets.OrderBy(b => b.From))
            {
                if (grossSalary > bracket.From)
                {
                    var taxable = Math.Min(grossSalary, bracket.To) - bracket.From;
                    tax += taxable * bracket.Rate;
                }
            }
            return Math.Round(tax, 2);
        }
    }
}
