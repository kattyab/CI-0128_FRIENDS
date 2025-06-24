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
                _deductionAggMock.Object);
        }

        /* ===== 1. Período mensual completo ===== */

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
                StartDate = new DateTime(2025, 5, 1)
            };

            var peticion = new PayrollRequest(
                "usuario@dummy.com",
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                new DateTime(2025, 5, 1),
                new DateTime(2025, 5, 30),
                "monthly");

            _daysWorkedMock
                .Setup(m => m.Calculate(It.IsAny<EmployeePayroll>(),
                                        It.IsAny<DateTime>(),
                                        It.IsAny<DateTime>()))
                .Returns(30);

            _salaryCalcMock
                .Setup(m => m.Calculate(It.IsAny<decimal>(),
                                        It.IsAny<int>(),
                                        It.IsAny<PayrollRequest>()))
                .Callback<decimal, int, PayrollRequest>((salary, days, req) => { })
                .Returns((2000m, 2000m));

            _salaryCalcMock
                .Setup(m => m.GetSalaryForDeductions(It.IsAny<EmployeePayroll>(),
                                                     It.IsAny<decimal>(),
                                                     It.IsAny<bool>()))
                .Returns(2000m);

            var apiDict = new Dictionary<string, decimal> { { "API1", 50m } };
            var benefitList = new List<BenefitDeductionResult>{
                                 new() { BenefitName="BEN1", DeductionValue=75m } };
            decimal ccss = 100m;
            decimal income = 150m;
            decimal total = 250m;

            _deductionAggMock
                .Setup(m => m.GetAllDeductionsAsync(It.IsAny<Guid>(),
                                                    It.IsAny<EmployeePayroll>(),
                                                    It.IsAny<decimal>(),
                                                    It.IsAny<bool>(),
                                                    It.IsAny<decimal>()))
                .ReturnsAsync((apiDict, benefitList, ccss, income, total));

            // Act
            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            // Verificaciones
            _daysWorkedMock.Verify(m => m.Calculate(It.IsAny<EmployeePayroll>(),
                                                    It.IsAny<DateTime>(),
                                                    It.IsAny<DateTime>()),
                                   Times.AtLeastOnce);

            _salaryCalcMock.Verify(m => m.Calculate(It.IsAny<decimal>(),
                                                    It.IsAny<int>(),
                                                    It.IsAny<PayrollRequest>()),
                                   Times.AtLeastOnce);

            Assert.AreEqual(empleado.EmpID, resumen.EmployeeId);
            Assert.AreEqual(empleado.ContractType, resumen.ContractType);
            Assert.AreEqual(empleado.RegistersHours, resumen.RegistersHours);
            Assert.AreEqual(2000m, resumen.GrossSalary);
            Assert.AreEqual(1750m, resumen.NetSalary);
            Assert.AreEqual(250m, resumen.TotalDeductions);
        }

        /* ===== 2. Período quincenal completo ===== */

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
                StartDate = new DateTime(2025, 6, 1)
            };

            var peticion = new PayrollRequest(
                "otro@dummy.com",
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 15),
                "biweekly");

            _daysWorkedMock.Setup(m => m.Calculate(empleado, peticion.Start, peticion.End))
                           .Returns(15);

            _salaryCalcMock.Setup(m => m.Calculate(empleado.BruteSalary, 15, peticion))
                           .Returns((1500m, 1500m));

            _salaryCalcMock.Setup(m => m.GetSalaryForDeductions(
                                        empleado, 1500m, true))
                           .Returns(1500m);

            var apiDict = new Dictionary<string, decimal> { { "API2", 30m } };
            var benefits = new List<BenefitDeductionResult>{
                                new() { BenefitName="BEN2", DeductionValue=60m } };
            decimal ccss = 80m;
            decimal renta = 100m;
            decimal total = 180m;

            _deductionAggMock.Setup(m => m.GetAllDeductionsAsync(
                                        peticion.CompanyId,
                                        empleado,
                                        1500m,
                                        true,
                                        1500m))
                             .ReturnsAsync((apiDict, benefits, ccss, renta, total));

            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1500m, resumen.GrossSalary);
                Assert.AreEqual(1500m - 180m, resumen.NetSalary);
                CollectionAssert.AreEqual(apiDict, resumen.ApiDeductions);
                CollectionAssert.AreEqual(benefits, resumen.BenefitDeductions);
            });
        }

        /* ===== 3. Período mensual parcial ===== */

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
                StartDate = new DateTime(2025, 7, 5)
            };

            var peticion = new PayrollRequest(
                "test@dummy.com",
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                new DateTime(2025, 7, 1),
                new DateTime(2025, 7, 30),
                "monthly");

            _daysWorkedMock.Setup(m => m.Calculate(empleado, peticion.Start, peticion.End))
                           .Returns(10);

            _salaryCalcMock.Setup(m => m.Calculate(empleado.BruteSalary, 10, peticion))
                           .Returns((1000m, 1000m));

            _salaryCalcMock.Setup(m => m.GetSalaryForDeductions(
                                        empleado, 1000m, false))
                           .Returns(800m);

            var apiDict = new Dictionary<string, decimal> { { "API3", 40m } };
            var benefits = new List<BenefitDeductionResult>{
                                new() { BenefitName="BEN3", DeductionValue=90m } };
            decimal ccss = 120m;
            decimal renta = 180m;
            decimal total = 340m;

            _deductionAggMock.Setup(m => m.GetAllDeductionsAsync(
                                        peticion.CompanyId,
                                        empleado,
                                        1000m,
                                        false,
                                        800m))
                             .ReturnsAsync((apiDict, benefits, ccss, renta, total));

            var resumen = await _calculator.CalculatePayrollAsync(empleado, peticion);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1000m, resumen.GrossSalary);
                Assert.AreEqual(660m, resumen.NetSalary);  // 1000 - 340
            });

            _deductionAggMock.Verify(m => m.GetAllDeductionsAsync(
                                        peticion.CompanyId,
                                        empleado,
                                        1000m,
                                        false,
                                        800m),
                                     Times.Once);
        }
    }
}
