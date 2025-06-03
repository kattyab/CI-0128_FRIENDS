using Kaizen.Server.Application.Services.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IDaysWorkedCalculator
    {
        int Calculate(EmployeePayroll employee, DateTime start, DateTime end);
    }

}
