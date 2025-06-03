using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface ISalaryCalculator
    {
        (decimal Gross, decimal Proportional) Calculate(decimal bruteSalary, int daysWorked, bool isBiweekly);
        decimal GetSalaryForDeductions(EmployeePayroll employee, decimal proportional, bool isBiweekly, bool isFullPeriod);
    }

}
