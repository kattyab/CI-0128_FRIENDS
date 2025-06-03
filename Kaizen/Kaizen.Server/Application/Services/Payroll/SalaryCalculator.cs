using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class SalaryCalculator : ISalaryCalculator
    {
        private const int BiweeklyPeriodDays = 15;
        private const int MonthlyPeriodDays = 30;

        public (decimal Gross, decimal Proportional) Calculate(decimal bruteSalary, int daysWorked, bool isBiweekly)
        {
            var totalDays = isBiweekly ? BiweeklyPeriodDays : MonthlyPeriodDays;
            var proportional = (bruteSalary / totalDays) * daysWorked;
            var gross = daysWorked == totalDays ? bruteSalary : proportional;
            return (gross, proportional);
        }

        public decimal GetSalaryForDeductions(EmployeePayroll employee, decimal proportional, bool isBiweekly, bool isFullPeriod)
        {
            return isFullPeriod
                ? (isBiweekly ? employee.BruteSalary * 2 : employee.BruteSalary)
                : (isBiweekly ? proportional * 2 : proportional);
        }
    }

}
