using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class SalaryCalculator : ISalaryCalculator
    {
        private const int BiweeklyPeriodDays = 15;
        private const int MonthlyPeriodDays = 30;
        private const string BiweeklyPayrollType = "Biweekly";
        private const string BiweeklyPayrollIdentifier = "biweekly";

        public (decimal Gross, decimal Proportional) Calculate(decimal bruteSalary, int daysWorked, PayrollRequest request)
        {
            var totalDays = (request.Type == BiweeklyPayrollIdentifier) ? BiweeklyPeriodDays : MonthlyPeriodDays;
            var proportional = (bruteSalary / totalDays) * daysWorked;
            var gross = daysWorked == totalDays ? bruteSalary : proportional;
            return (gross, proportional);
        }

        public decimal GetSalaryForDeductions(EmployeePayroll employee, decimal proportional, bool isFullPeriod)
        {
            return isFullPeriod
                ? ((employee.PayrollTypeDescription == BiweeklyPayrollType) ? employee.BruteSalary * 2 : employee.BruteSalary)
                : ((employee.PayrollTypeDescription == BiweeklyPayrollType) ? proportional * 2 : proportional);
        }
    }

}
