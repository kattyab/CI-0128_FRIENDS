using Moq;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Application.Services.CCSS;
using Kaizen.Server.Application.Dtos.CCSS;

namespace Tests.CCSSCalculatorTest
{
    [TestFixture]
    public class CCSSCalculatorTest
    {
        private Mock<ICCSSRateProvider> _mockRateProvider;
        private CCSSCalculator _ccssCalculator;

        private const decimal SEM_RATE = 0.055m;
        private const decimal IVM_RATE = 0.0417m;
        private const decimal LPT_RATE = 0.01m;

        private const int SecondDecimal = 2;

        [SetUp]
        public void Setup()
        {
            _mockRateProvider = new Mock<ICCSSRateProvider>();
            _ccssCalculator = new CCSSCalculator(_mockRateProvider.Object);
        }

        private void SetupRates(decimal sem, decimal ivm, decimal lpt)
        {
            _mockRateProvider.Setup(r => r.GetRates()).Returns(new CCSSRates
            {
                SEM = sem,
                IVM = ivm,
                LPT = lpt
            });
        }

        [Test]
        public void GivenZeroGrossSalaryWhenCalculateDeductionThenReturnsZero()
        {
            SetupRates(SEM_RATE, IVM_RATE, LPT_RATE);
            decimal grossSalary = 0m;

            var deduction = _ccssCalculator.CalculateDeduction(grossSalary);

            Assert.AreEqual(0m, deduction);
        }

        [Test]
        public void GivenValidGrossSalaryWhenCalculateDeductionThenReturnsCorrectAmount()
        {
            SetupRates(SEM_RATE, IVM_RATE, LPT_RATE);
            decimal grossSalary = 5_500_020m;
            decimal expectedRate = SEM_RATE + IVM_RATE + LPT_RATE;
            decimal expectedDeduction = Math.Round(grossSalary * expectedRate, SecondDecimal, MidpointRounding.ToEven);

            var deduction = _ccssCalculator.CalculateDeduction(grossSalary);

            Assert.AreEqual(expectedDeduction, deduction);
        }

        [Test]
        public void GivenGrossSalaryWithRoundingEdgeCaseWhenCalculateDeductionThenRoundsToEven()
        {
            SetupRates(SEM_RATE, IVM_RATE, LPT_RATE);
            decimal grossSalary = 1234.5678m;
            decimal expectedRate = SEM_RATE + IVM_RATE + LPT_RATE;
            decimal expectedDeduction = Math.Round(grossSalary * expectedRate, SecondDecimal, MidpointRounding.ToEven);

            var deduction = _ccssCalculator.CalculateDeduction(grossSalary);

            Assert.AreEqual(expectedDeduction, deduction);
        }

        [Test]
        public void GivenDifferentRatesWhenCalculateDeductionThenReturnsDifferentResult()
        {
            SetupRates(0.054m, 0.0418m, 0.02m);
            decimal grossSalary = 1234.5678m;

            decimal expectedDeductionWithOldRate = Math.Round(grossSalary * (SEM_RATE + IVM_RATE + LPT_RATE), SecondDecimal, MidpointRounding.ToEven);
            var deduction = _ccssCalculator.CalculateDeduction(grossSalary);

            Assert.AreNotEqual(expectedDeductionWithOldRate, deduction);
        }
    }
}
