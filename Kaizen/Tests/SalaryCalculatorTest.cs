using NUnit.Framework;
using Kaizen.Server.Application.Services.Payroll;
using Kaizen.Server.Application.Dtos.Payroll;
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

        [Test]
        public void Calculate_BiweeklyFullPeriod_ReturnsGrossEqualBruteAndProportionalEqualBrute()
        {
            decimal bruteSalary = 1500m;
            int daysWorked = 15;
            bool isBiweekly = true;

            var result = _calculator.Calculate(bruteSalary, daysWorked, isBiweekly);

            Assert.AreEqual(bruteSalary, result.Gross);
            Assert.AreEqual(bruteSalary, result.Proportional);
        }

        [Test]
        public void Calculate_BiweeklyPartialPeriod_ReturnsProportionalAndGrossEqualProportional()
        {
            decimal bruteSalary = 1500m;
            int daysWorked = 5;
            bool isBiweekly = true;
            decimal expectedProportional = (bruteSalary / 15m) * daysWorked;

            var result = _calculator.Calculate(bruteSalary, daysWorked, isBiweekly);

            Assert.AreEqual(expectedProportional, result.Proportional);
            Assert.AreEqual(expectedProportional, result.Gross);
        }

        [Test]
        public void Calculate_MonthlyFullPeriod_ReturnsGrossEqualBruteAndProportionalEqualBrute()
        {
            decimal bruteSalary = 3000m;
            int daysWorked = 30;
            bool isBiweekly = false;

            var result = _calculator.Calculate(bruteSalary, daysWorked, isBiweekly);

            Assert.AreEqual(bruteSalary, result.Gross);
            Assert.AreEqual(bruteSalary, result.Proportional);
        }

        [Test]
        public void Calculate_MonthlyPartialPeriod_ReturnsProportionalAndGrossEqualProportional()
        {
            decimal bruteSalary = 3000m;
            int daysWorked = 10;
            bool isBiweekly = false;
            decimal expectedProportional = (bruteSalary / 30m) * daysWorked;

            var result = _calculator.Calculate(bruteSalary, daysWorked, isBiweekly);

            Assert.AreEqual(expectedProportional, result.Proportional);
            Assert.AreEqual(expectedProportional, result.Gross);
        }

        [Test]
        public void GetSalaryForDeductions_BiweeklyFullPeriod_ReturnsBruteTimesTwo()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 1500m
            };
            decimal proportional = 1500m;
            bool isBiweekly = true;
            bool isFullPeriod = true;
            decimal expected = employee.BruteSalary * 2m;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isBiweekly, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_MonthlyFullPeriod_ReturnsBrute()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 3000m
            };
            decimal proportional = 3000m;
            bool isBiweekly = false;
            bool isFullPeriod = true;
            decimal expected = employee.BruteSalary;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isBiweekly, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_BiweeklyPartialPeriod_ReturnsProportionalTimesTwo()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 1500m
            };
            decimal proportional = 500m;
            bool isBiweekly = true;
            bool isFullPeriod = false;
            decimal expected = proportional * 2m;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isBiweekly, isFullPeriod);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetSalaryForDeductions_MonthlyPartialPeriod_ReturnsProportional()
        {
            var employee = new EmployeePayroll
            {
                BruteSalary = 3000m
            };
            decimal proportional = 1000m;
            bool isBiweekly = false;
            bool isFullPeriod = false;
            decimal expected = proportional;

            decimal result = _calculator.GetSalaryForDeductions(employee, proportional, isBiweekly, isFullPeriod);

            Assert.AreEqual(expected, result);
        }
    }
}

