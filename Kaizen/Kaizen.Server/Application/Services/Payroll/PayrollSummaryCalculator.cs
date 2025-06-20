using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class PayrollSummaryCalculator : IPayrollSummaryCalculator
    {
        private const int DefaultMonthlyDays = 30;
        private const int DefaultBiweeklyDays = 15;
        private readonly IDaysWorkedCalculator _daysWorkedCalculator;
        private readonly ISalaryCalculator _salaryCalculator;
        private readonly IDeductionAggregator _deductionAggregator;

        public PayrollSummaryCalculator(
            IDaysWorkedCalculator daysWorkedCalculator,
            ISalaryCalculator salaryCalculator,
            IDeductionAggregator deductionAggregator)
        {
            _daysWorkedCalculator = daysWorkedCalculator;
            _salaryCalculator = salaryCalculator;
            _deductionAggregator = deductionAggregator;
        }

        public async Task<PayrollSummary> CalculatePayrollAsync(EmployeePayroll employee, PayrollRequest request)
        {
            var daysWorked = _daysWorkedCalculator.Calculate(employee, request.Start, request.End);
            var isBiweekly = employee.PayrollTypeDescription.Equals("Biweekly", StringComparison.OrdinalIgnoreCase);
            var totalDays = isBiweekly ? DefaultBiweeklyDays : DefaultMonthlyDays;
            var isFullPeriod = daysWorked == totalDays;

            var (gross, proportional) = _salaryCalculator.Calculate(employee.BruteSalary, daysWorked, request);
            var salaryForDeductions = _salaryCalculator.GetSalaryForDeductions(employee, proportional, isFullPeriod);

            var (api, benefit, ccss, income, total) = await _deductionAggregator.GetAllDeductionsAsync(
                request.CompanyId, employee, proportional, isFullPeriod, salaryForDeductions);

            return new PayrollSummary
            {
                EmployeeId = employee.EmpID,
                ContractType = employee.ContractType,
                RegistersHours = employee.RegistersHours,
                GrossSalary = gross,
                NetSalary = gross - total,
                TotalDeductions = total,
                ApiDeductions = api,
                BenefitDeductions = benefit,
                CCSSDeduction = ccss,
                IncomeTax = income
            };
        }
    }

}
