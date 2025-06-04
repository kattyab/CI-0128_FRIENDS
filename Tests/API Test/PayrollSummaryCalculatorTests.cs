using Moq;
using Kaizen.Server.Application.Interfaces.Payroll;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.API.Controllers;

namespace Kaizen.Server.Tests.Payroll
{
    [TestFixture]
    public class PayrollSummaryCalculatorTests
    {
        private Mock<IDaysWorkedCalculator> _daysWorkedMock;
        private Mock<ISalaryCalculator> _salaryCalcMock;
        private Mock<IDeductionAggregator> _deductionAggMock;
        private PayrollSummaryCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _daysWorkedMock = new Mock<IDaysWorkedCalculator>();
            _salaryCalcMock = new Mock<ISalaryCalculator>();
            _deductionAggMock = new Mock<IDeductionAggregator>();

            _calculator = new PayrollSummaryCalculator(
                _daysWorkedMock.Object,
                _salaryCalcMock.Object,
                _deductionAggMock.Object
            );
        }

        [Test]
        public async Task CalculatePayrollAsync_MonthlyFullPeriod_ReturnsExpectedSummary()
        {
            var empleado = new EmployeePayroll
            {
                EmpID = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ContractType = "FullTime",
                RegistersHours = true,
                BruteSalary = 2000m,
                PayrollTypeDescription = "Monthly",
                StartDate = new DateTime(2025, 5, 1),
                FireDate = null
            };

            var peticion = new PayrollRequest(
                "usuario@dummy.com",
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                new DateTime(2025, 5, 1),
                new DateTime(2025, 5, 30),
                "monthly"
            );

            _daysWorkedMock
                .Setup(m => m.Calculate(It.IsAny<EmployeePayroll>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Callback<EmployeePayroll, DateTime, DateTime>((emp, start, end) =>
                {})
                .Returns(30);

            _salaryCalcMock
                .Setup(m => m.Calculate(It.IsAny<decimal>(), It.IsAny<int>(), peticion))
                .Callback<decimal, int, PayrollRequest>((salary, days, request) => { })
                .Returns((2000m, 2000m));

            _salaryCalcMock
                .Setup(m => m.GetSalaryForDeductions(It.IsAny<EmployeePayroll>(), It.IsAny<decimal>(), It.IsAny<bool>()))
                .Returns(2000m);

            var apiDict = new Dictionary<string, decimal> { { "API1", 50m } };
            var benefitList = new List<BenefitDeductionResult>
            {
                new BenefitDeductionResult { BenefitName = "BEN1", DeductionValue = 75m }
            };

            decimal ccssValue = 100m;
            decimal incomeValue = 150m;
            decimal totalValue = 250m;

            _deductionAggMock
                .Setup(m => m.GetAllDeductionsAsync(It.IsAny<Guid>(), It.IsAny<EmployeePayroll>(), It.IsAny<decimal>(), It.IsAny<bool>(), It.IsAny<decimal>()))
                .ReturnsAsync((apiDict, benefitList, ccssValue, incomeValue, totalValue));

            // Act
            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            _daysWorkedMock.Verify(m => m.Calculate(It.IsAny<EmployeePayroll>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.AtLeastOnce);
            _salaryCalcMock.Verify(m => m.Calculate(It.IsAny<decimal>(), It.IsAny<int>(), peticion), Times.AtLeastOnce);

            Assert.AreEqual(empleado.EmpID, resumen.EmployeeId);
            Assert.AreEqual(empleado.ContractType, resumen.ContractType);
            Assert.AreEqual(empleado.RegistersHours, resumen.RegistersHours);
            Assert.AreEqual(2000m, resumen.GrossSalary);
            Assert.AreEqual(1750m, resumen.NetSalary);
            Assert.AreEqual(250m, resumen.TotalDeductions);
        }

        [Test]
        public async Task CalculatePayrollAsync_BiweeklyFullPeriod_ReturnsExpectedSummary()
        {
            var empleado = new EmployeePayroll
            {
                EmpID = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                ContractType = "PartTime",
                RegistersHours = false,
                BruteSalary = 1500m,
                PayrollTypeDescription = "Biweekly",
                StartDate = new DateTime(2025, 6, 1),
                FireDate = null
            };
            var peticion = new PayrollRequest(
                "otro@dummy.com",
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 15),
                "biweekly"
            );

            _daysWorkedMock
                .Setup(m => m.Calculate(empleado, peticion.Start, peticion.End))
                .Returns(15);

            _salaryCalcMock
                .Setup(m => m.Calculate(
                    empleado.BruteSalary,
                    15,
                    peticion))
                .Returns((1500m, 1500m));

            _salaryCalcMock
                .Setup(m => m.GetSalaryForDeductions(
                    empleado,
                    1500m,
                    true))
                .Returns(1500m);

            var apiDict2 = new Dictionary<string, decimal>
            {
                { "API2", 30m }
            };
            var benefitList2 = new List<BenefitDeductionResult>
            {
                new BenefitDeductionResult
                {
                    BenefitName = "BEN2",
                    DeductionValue = 60m
                }
            };
            decimal ccss2 = 80m;
            decimal income2 = 100m;
            decimal total2 = 180m;

            _deductionAggMock
                .Setup(m => m.GetAllDeductionsAsync(
                    peticion.CompanyId,
                    empleado,
                    1500m,
                    true,
                    1500m))
                .ReturnsAsync((apiDict2, benefitList2, ccss2, income2, total2));

            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            Assert.AreEqual(empleado.EmpID, resumen.EmployeeId);
            Assert.AreEqual(empleado.ContractType, resumen.ContractType);
            Assert.AreEqual(empleado.RegistersHours, resumen.RegistersHours);
            Assert.AreEqual(1500m, resumen.GrossSalary);
            Assert.AreEqual(1500m - 180m, resumen.NetSalary);
            Assert.AreEqual(180m, resumen.TotalDeductions);
            CollectionAssert.AreEqual(apiDict2, resumen.ApiDeductions);
            CollectionAssert.AreEqual(benefitList2, resumen.BenefitDeductions);
            Assert.AreEqual(ccss2, resumen.CCSSDeduction);
            Assert.AreEqual(income2, resumen.IncomeTax);
        }

        [Test]
        public async Task CalculatePayrollAsync_MonthlyPartialPeriod_IsFullPeriodFalseAndCorrectParams()
        {
            var empleado = new EmployeePayroll
            {
                EmpID = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                ContractType = "Contractor",
                RegistersHours = true,
                BruteSalary = 3000m,
                PayrollTypeDescription = "Monthly",
                StartDate = new DateTime(2025, 7, 5),
                FireDate = null
            };
            var peticion = new PayrollRequest(
                "test@dummy.com",
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                new DateTime(2025, 7, 1),
                new DateTime(2025, 7, 30),
                "monthly"
            );

            _daysWorkedMock
                .Setup(m => m.Calculate(empleado, peticion.Start, peticion.End))
                .Returns(10);

            _salaryCalcMock
                .Setup(m => m.Calculate(
                    empleado.BruteSalary,
                    10,
                    peticion))
                .Returns((1000m, 1000m));

            _salaryCalcMock
                .Setup(m => m.GetSalaryForDeductions(
                    empleado,
                    1000m,
                    false))
                .Returns(800m);

            var apiDict3 = new Dictionary<string, decimal>
            {
                { "API3", 40m }
            };
            var benefitList3 = new List<BenefitDeductionResult>
            {
                new BenefitDeductionResult
                {
                    BenefitName = "BEN3",
                    DeductionValue = 90m
                }
            };
            decimal ccss3 = 120m;
            decimal income3 = 180m;
            decimal total3 = 340m;

            _deductionAggMock
                .Setup(m => m.GetAllDeductionsAsync(
                    peticion.CompanyId,
                    empleado,
                    1000m,
                    false,
                    800m))
                .ReturnsAsync((apiDict3, benefitList3, ccss3, income3, total3));

            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            Assert.AreEqual(empleado.EmpID, resumen.EmployeeId);
            Assert.AreEqual(1000m, resumen.GrossSalary);
            Assert.AreEqual(1000m - 340m, resumen.NetSalary);
            Assert.AreEqual(340m, resumen.TotalDeductions);

            _deductionAggMock.Verify(m => m.GetAllDeductionsAsync(
                peticion.CompanyId,
                empleado,
                1000m,
                false,
                800m),
                Times.Once);

            CollectionAssert.AreEqual(apiDict3, resumen.ApiDeductions);
            CollectionAssert.AreEqual(benefitList3, resumen.BenefitDeductions);
            Assert.AreEqual(ccss3, resumen.CCSSDeduction);
            Assert.AreEqual(income3, resumen.IncomeTax);
        }
    }
}
