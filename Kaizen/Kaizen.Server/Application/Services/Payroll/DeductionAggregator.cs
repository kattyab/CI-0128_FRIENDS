using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Dtos.Payroll;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Application.Interfaces.IncomeTax;
using Kaizen.Server.Application.Interfaces.Payroll;

namespace Kaizen.Server.Application.Services.Payroll
{
    public class DeductionAggregator : IDeductionAggregator
    {
        private const decimal BiweeklyFactor = 2m;

        private readonly IApiDeductionServiceFactory _apiFactory;
        private readonly IBenefitDeductionServiceFactory _benefitFactory;
        private readonly ICCSSCalculator _ccssCalculator;
        private readonly IIncomeTaxCalculator _incomeTaxCalculator;

        public DeductionAggregator(
            IApiDeductionServiceFactory apiFactory,
            IBenefitDeductionServiceFactory benefitFactory,
            ICCSSCalculator ccssCalculator,
            IIncomeTaxCalculator incomeTaxCalculator)
        {
            _apiFactory = apiFactory;
            _benefitFactory = benefitFactory;
            _ccssCalculator = ccssCalculator;
            _incomeTaxCalculator = incomeTaxCalculator;
        }

        public async Task<(Dictionary<string, decimal>, List<BenefitDeductionResult>, decimal, decimal, decimal)>
            GetAllDeductionsAsync(Guid companyId, EmployeePayroll employee, decimal proportionalSalary,
                bool isFullPeriod, bool isBiweekly, decimal salaryForDeductions)
        {
            var apiService = _apiFactory.Create(companyId);
            var benefitService = _benefitFactory.Create(companyId);

            var apiDeductions = await apiService.GetDeductionsForEmployeeAsync(employee.EmpID);
            var benefitDeductions = isFullPeriod
                ? await benefitService.GetBenefitDeductionsForEmployeeAsync(employee.EmpID)
                : await benefitService.GetBenefitDeductionsForEmployeeAsync(employee.EmpID, proportionalSalary);

            if (isBiweekly)
            {
                apiDeductions = apiDeductions.ToDictionary(x => x.Key, x => x.Value / BiweeklyFactor);
                foreach (var benefit in benefitDeductions)
                    benefit.DeductionValue /= BiweeklyFactor;
            }
            decimal ccss = CalculateCCSSDeduction(employee, salaryForDeductions);
            decimal income = CalculateIncomeTaxDeduction(employee, salaryForDeductions);

            if (isBiweekly)
            {
                ccss /= BiweeklyFactor;
                income /= BiweeklyFactor;
            }

            var total = apiDeductions.Values.Sum() + benefitDeductions.Sum(x => x.DeductionValue) + ccss + income;

            return (apiDeductions, benefitDeductions, ccss, income, total);
        }

        private decimal CalculateIncomeTaxDeduction(EmployeePayroll employee, decimal salaryForDeductions)
        {
            return employee.ContractType == "Servicios Profesionales" ? 0m : _incomeTaxCalculator.Calculate(salaryForDeductions);
        }

        private decimal CalculateCCSSDeduction(EmployeePayroll employee, decimal salaryForDeductions)
        {
            return employee.ContractType == "Servicios Profesionales" ? 0m : _ccssCalculator.CalculateDeduction(salaryForDeductions);
        }
    }

}
