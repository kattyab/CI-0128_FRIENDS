using NUnit.Framework;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.API.Controllers;
using System;

namespace Kaizen.Server.Tests.Payroll
{
    [TestFixture]
    public class SalaryCalculatorTests
    {
        private SalaryCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new SalaryCalculator();
        }

        /* ===== Cálculos: Bi-semanal ===== */

        [Test]
        public void Calculate_BiweeklyFullPeriod_ReturnsGrossEqualBruteAndProportionalEqualBrute()
        {
            decimal bruteSalary = 1500m;
            int daysWorked = 15;

            var request = new PayrollRequest(
                "unit@test.com",
                Guid.Empty,
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 15),
                "biweekly");

            var result = _calculator.Calculate(bruteSalary, daysWorked, request);

            Assert.AreEqual(bruteSalary, result.Gross);
            Assert.AreEqual(bruteSalary, result.Proportional);
        }

        [Test]
        public void Calculate_BiweeklyPartialPeriod_ReturnsProportionalAndGrossEqualProportional()
        {
            decimal bruteSalary = 1500m;
            int daysWorked = 5;
            decimal expectedProportional = (bruteSalary / 15m) * daysWorked;

            var request = new PayrollRequest(
                "unit@test.com",
                Guid.Empty,
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 15),
                "biweekly");

            var result = _calculator.Calculate(bruteSalary, daysWorked, request);

            Assert.AreEqual(expectedProportional, result.Proportional);
            Assert.AreEqual(expectedProportional, result.Gross);
        }

        /* ===== Cálculos: Mensual ===== */

        [Test]
        public void Calculate_MonthlyFullPeriod_ReturnsGrossEqualBruteAndProportionalEqualBrute()
        {
            decimal bruteSalary = 3000m;
            int daysWorked = 30;

            var request = new PayrollRequest(
                "unit@test.com",
                Guid.Empty,
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 30),
                "monthly");

            var result = _calculator.Calculate(bruteSalary, daysWorked, request);

            Assert.AreEqual(bruteSalary, result.Gross);
            Assert.AreEqual(bruteSalary, result.Proportional);
        }

        [Test]
        public void Calculate_MonthlyPartialPeriod_ReturnsProportionalAndGrossEqualProportional()
        {
            decimal bruteSalary = 3000m;
            int daysWorked = 10;
            decimal expectedProportional = (bruteSalary / 30m) * daysWorked;

            var request = new PayrollRequest(
                "unit@test.com",
                Guid.Empty,
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 30),
                "monthly");

            var result = _calculator.Calculate(bruteSalary, daysWorked, request);

            Assert.AreEqual(expectedProportional, result.Proportional);
            Assert.AreEqual(expectedProportional, result.Gross);
        }

        /* ===== Salario para deducciones ===== */

        [Test]
        public void GetSalaryForDeductions_BiweeklyFullPeriod_ReturnsBruteTimesTwo()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 1500m,
                PayrollTypeDescription = "Biweekly"
            };
            decimal proportional = 1500m;
            bool isFullPeriod = true;
            decimal expected = employee.BruteSalary * 2m;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_MonthlyFullPeriod_ReturnsBrute()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 3000m,
                PayrollTypeDescription = "Monthly"
            };
            decimal proportional = 3000m;
            bool isFullPeriod = true;
            decimal expected = employee.BruteSalary;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_BiweeklyPartialPeriod_ReturnsProportionalTimesTwo()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 1500m,
                PayrollTypeDescription = "Biweekly"
            };
            decimal proportional = 500m;
            bool isFullPeriod = false;
            decimal expected = proportional * 2m;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_MonthlyPartialPeriod_ReturnsProportional()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 3000m,
                PayrollTypeDescription = "Monthly"
            };
            decimal proportional = 1000m;
            bool isFullPeriod = false;
            decimal expected = proportional;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isFullPeriod);

            Assert.AreEqual(expected, result);
        }
    }
}
