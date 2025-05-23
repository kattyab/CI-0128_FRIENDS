using Moq;
using Kaizen.Server.Infrastructure.Services.IncomeTax;
using Kaizen.Server.Infrastructure.Helpers.IncomeTax;

namespace Tests.IncomeTaxCalculatorTest
{
    [TestFixture]
    public class IncomeTaxCalculatorTest
    {
        private Mock<IIncomeTaxBracketProvider> _mockBracketProvider;
        private IncomeTaxCalculator _calculator;

        private const decimal Zero = 0m;
        private const decimal Bracket1Threshold = 922_000m;
        private const decimal Bracket2Threshold = 1_352_000m;
        private const decimal Bracket3Threshold = 2_373_000m;
        private const decimal Bracket4Threshold = 4_745_000m;

        private const decimal Rate0 = 0.00m;
        private const decimal Rate1 = 0.10m;
        private const decimal Rate2 = 0.15m;
        private const decimal Rate3 = 0.20m;
        private const decimal Rate4 = 0.25m;

        private const int SecondDecimal = 2;

        [SetUp]
        public void SetUp()
        {
            _mockBracketProvider = new Mock<IIncomeTaxBracketProvider>();
            _mockBracketProvider.Setup(p => p.GetBrackets()).Returns(GetDefaultBrackets());
            _calculator = new IncomeTaxCalculator(_mockBracketProvider.Object);
        }

        [TestCase(900_000)]
        [TestCase(1_000_000)]
        [TestCase(2_000_000)]
        [TestCase(3_000_000)]
        [TestCase(5_000_000)]
        public void CalculateTaxForSalary(decimal grossSalary)
        {
            decimal expectedTax = CalculateExpectedTax(grossSalary);
            decimal calculatedTax = _calculator.Calculate(grossSalary);
            Assert.AreEqual(expectedTax, calculatedTax);
        }

        private decimal CalculateExpectedTax(decimal salary)
        {
            decimal tax = 0;
            tax += CalculateTaxSegment(salary, Bracket1Threshold, Bracket2Threshold, Rate1);
            tax += CalculateTaxSegment(salary, Bracket2Threshold, Bracket3Threshold, Rate2);
            tax += CalculateTaxSegment(salary, Bracket3Threshold, Bracket4Threshold, Rate3);
            tax += CalculateTaxSegment(salary, Bracket4Threshold, decimal.MaxValue, Rate4);
            return Math.Round(tax, SecondDecimal);
        }

        private decimal CalculateTaxSegment(decimal salary, decimal lowerLimit, decimal upperLimit, decimal rate)
        {
            if (salary <= lowerLimit)
                return 0;
            decimal taxableAmount = Math.Min(salary, upperLimit) - lowerLimit;
            return taxableAmount * rate;
        }

        private List<IncomeTaxBracket> GetDefaultBrackets()
        {
            return new List<IncomeTaxBracket>
            {
                new IncomeTaxBracket { From = Zero, To = Bracket1Threshold, Rate = Rate0 },
                new IncomeTaxBracket { From = Bracket1Threshold, To = Bracket2Threshold, Rate = Rate1 },
                new IncomeTaxBracket { From = Bracket2Threshold, To = Bracket3Threshold, Rate = Rate2 },
                new IncomeTaxBracket { From = Bracket3Threshold, To = Bracket4Threshold, Rate = Rate3 },
                new IncomeTaxBracket { From = Bracket4Threshold, To = decimal.MaxValue, Rate = Rate4 }
            };
        }
    }
}