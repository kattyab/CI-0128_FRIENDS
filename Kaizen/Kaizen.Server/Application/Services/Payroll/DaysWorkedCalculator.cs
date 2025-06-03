using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class DaysWorkedCalculator : IDaysWorkedCalculator
    {
        private const int MaxDays = 30;

        public int Calculate(EmployeePayroll employee, DateTime payrollPeriodStart, DateTime payrollPeriodEnd)
        {
            var start = employee.StartDate > payrollPeriodStart ? employee.StartDate : payrollPeriodStart;
            var end = employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodEnd
                ? employee.FireDate.Value : payrollPeriodEnd;

            if (start > payrollPeriodEnd || (employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodStart))
                return 0;

            var daysWorked = (end - start).Days + 1;
            return Math.Max(0, Math.Min(daysWorked, MaxDays));
        }
    }

}
