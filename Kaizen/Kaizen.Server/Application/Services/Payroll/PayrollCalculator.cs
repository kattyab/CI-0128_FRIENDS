using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Application.Interfaces.IncomeTax;


namespace Kaizen.Server.Application.Services.Payroll
{
    public class PayrollCalculator
    {
        private readonly IApiDeductionServiceFactory _apiDeductionServiceFactory;
        private readonly IBenefitDeductionServiceFactory _benefitDeductionServiceFactory;
        private readonly ICCSSCalculator _ccssCalculator;
        private readonly IIncomeTaxCalculator _incomeTaxCalculator;

        public PayrollCalculator(
            IApiDeductionServiceFactory apiDeductionServiceFactory,
            IBenefitDeductionServiceFactory benefitDeductionServiceFactory,
            ICCSSCalculator ccssCalculator,
            IIncomeTaxCalculator incomeTaxCalculator)
        {
            _apiDeductionServiceFactory = apiDeductionServiceFactory;
            _benefitDeductionServiceFactory = benefitDeductionServiceFactory;
            _ccssCalculator = ccssCalculator;
            _incomeTaxCalculator = incomeTaxCalculator;
        }

        public (IApiDeductionService ApiDeductionService, IBenefitDeductionService BenefitDeductionService) CreateDeductionServices(Guid companyId)
        {
            var apiDeductionService = _apiDeductionServiceFactory.Create(companyId);
            var benefitDeductionService = _benefitDeductionServiceFactory.Create(companyId);
            return (apiDeductionService, benefitDeductionService);
        }

        public async Task<PayrollSummary> CalculatePayrollAsync(
            EmployeePayroll employee,
            IApiDeductionService apiDeductionService,
            IBenefitDeductionService benefitDeductionService)
        {
            var isBiweekly = employee.PayrollTypeDescription.Equals("Biweekly", StringComparison.OrdinalIgnoreCase);

            var apiDeductions = await apiDeductionService.GetDeductionsForEmployeeAsync(employee.EmpID);
            var benefitDeductions = await benefitDeductionService.GetBenefitDeductionsForEmployeeAsync(employee.EmpID);

            if (isBiweekly)
            {
                apiDeductions = AdjustDeductionsForBiweekly(apiDeductions, benefitDeductions);
            }

            decimal salaryForDeductions = isBiweekly ? employee.BruteSalary * 2 : employee.BruteSalary;

            var ccssDeduction = GetCcssDeduction(employee, salaryForDeductions);
            var incomeTaxDeduction = GetIncomeTaxDeduction(employee, salaryForDeductions);

            if (isBiweekly)
            {
                ccssDeduction /= 2m;
                incomeTaxDeduction /= 2m;
            }

            var totalDeductions = GetTotalDeductions(apiDeductions, benefitDeductions, ccssDeduction, incomeTaxDeduction);

            var netSalary = employee.BruteSalary - totalDeductions;

            return new PayrollSummary
            {
                EmployeeId = employee.EmpID,
                ContractType = employee.ContractType,
                RegistersHours = employee.RegistersHours,
                GrossSalary = employee.BruteSalary,
                NetSalary = netSalary,
                TotalDeductions = totalDeductions,
                ApiDeductions = apiDeductions,
                BenefitDeductions = benefitDeductions,
                CCSSDeduction = ccssDeduction,
                IncomeTax = incomeTaxDeduction
            };
        }

        private static decimal GetTotalDeductions(Dictionary<string, decimal> apiDeductions, List<Dtos.BenefitDeductions.BenefitDeductionResult> benefitDeductions, decimal ccssDeduction, decimal incomeTaxDeduction)
        {
            return apiDeductions.Values.Sum()
                                             + benefitDeductions.Sum(benefitDeduction => benefitDeduction.DeductionValue)
                                             + ccssDeduction + incomeTaxDeduction;
        }

        private decimal GetIncomeTaxDeduction(EmployeePayroll employee, decimal salaryForDeductions)
        {
            return employee.ContractType == "Servicios Profesionales" ? 0m : _incomeTaxCalculator.Calculate(salaryForDeductions);
        }

        private decimal GetCcssDeduction(EmployeePayroll employee, decimal salaryForDeductions)
        {
            return employee.ContractType == "Servicios Profesionales" ? 0m : _ccssCalculator.CalculateDeduction(salaryForDeductions);
        }

        private static Dictionary<string, decimal> AdjustDeductionsForBiweekly(Dictionary<string, decimal> apiDeductions, List<Dtos.BenefitDeductions.BenefitDeductionResult> benefitDeductions)
        {
            apiDeductions = apiDeductions.ToDictionary(
                                deductionEntry => deductionEntry.Key,
                                deductionEntry => deductionEntry.Value / 2m
                            );

            foreach (var benefitDeduction in benefitDeductions)
            {
                benefitDeduction.DeductionValue /= 2m;
            }

            return apiDeductions;
        }
    }
}
