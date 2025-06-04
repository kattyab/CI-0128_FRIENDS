using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos.Payroll;

namespace Kaizen.Server.Application.Interfaces.Payroll
{
    public interface IDeductionAggregator
    {
        Task<(Dictionary<string, decimal>, List<BenefitDeductionResult>, decimal ccss, decimal income, decimal total)>
            GetAllDeductionsAsync(Guid companyId, EmployeePayroll employee, decimal proportionalSalary,
            bool isFullPeriod, decimal salaryForDeductions);
    }

}
