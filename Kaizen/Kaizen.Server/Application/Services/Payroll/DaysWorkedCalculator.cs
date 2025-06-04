using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class DaysWorkedCalculator : IDaysWorkedCalculator
    {
        private const int DaysInMonth = 30;

        public int Calculate(EmployeePayroll employee, DateTime payrollPeriodStart, DateTime payrollPeriodEnd)
        {
            var start = employee.StartDate > payrollPeriodStart ? employee.StartDate : payrollPeriodStart;
            var end = employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodEnd
                ? employee.FireDate.Value : payrollPeriodEnd;

            if (start > payrollPeriodEnd || WasEmployeeFiredPriorToPeriod(employee, payrollPeriodStart))
                return 0;

            var daysWorked = CalculatePayrollDays(start, end);
            return Math.Max(0, Math.Min(daysWorked, DaysInMonth));
        }

        private int CalculatePayrollDays(DateTime start, DateTime end)
        {
            var startPayrollDay = GetPayrollDayOfMonth(start);
            var endPayrollDay = GetPayrollDayOfMonth(end);

            if (start.Year == end.Year && start.Month == end.Month)
            {
                return endPayrollDay - startPayrollDay + 1;
            }

            int totalDays = 0;

            totalDays += DaysInMonth - startPayrollDay + 1;

            var currentDate = new DateTime(start.Year, start.Month, 1).AddMonths(1);
            var endDate = new DateTime(end.Year, end.Month, 1);

            while (currentDate < endDate)
            {
                totalDays += DaysInMonth;
                currentDate = currentDate.AddMonths(1);
            }

            if (start.Month != end.Month || start.Year != end.Year)
            {
                totalDays += endPayrollDay;
            }

            return totalDays;
        }

        private int GetPayrollDayOfMonth(DateTime date)
        {
            return Math.Min(date.Day, DaysInMonth);
        }

        private static bool WasEmployeeFiredPriorToPeriod(EmployeePayroll employee, DateTime payrollPeriodStart)
        {
            return (employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodStart);
        }
    }

}
