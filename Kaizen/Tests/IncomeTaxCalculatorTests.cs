using Kaizen.Server.Infrastructure.Services.IncomeTax;

namespace Tests
{
    public class IncomeTaxCalculatorTests
    {
        private IncomeTaxCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new IncomeTaxCalculator();
        }

        [Test]
        public void Calculate_WhenSalaryBelow922000_ReturnsZero()
        {
            var salary = 900_000m;
            var tax = _calculator.Calculate(salary);
            Assert.AreEqual(0, tax);
        }

        [Test]
        public void Calculate_WhenSalaryBetween922001And1352000_ReturnsCorrectTax()
        {
            var salary = 1_000_000m;
            var expectedTax = (salary - 922_000) * 0.1m;
            var tax = _calculator.Calculate(salary);
            Assert.AreEqual(expectedTax, tax);
        }
    }
}