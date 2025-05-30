﻿using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Infrastructure.Helpers.IncomeTax;

namespace Kaizen.Server.Application.Services.IncomeTax
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly List<IncomeTaxBracket> _brackets;

        private const int SecondDecimal = 2;

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
            return Math.Round(tax, SecondDecimal, MidpointRounding.ToEven);
        }
    }
}
