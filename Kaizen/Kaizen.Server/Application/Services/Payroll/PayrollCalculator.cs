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
            Guid employeeId,
            decimal grossSalary,
            string contractType,
            bool registersHours,
            IApiDeductionService apiDeductionService,
            IBenefitDeductionService benefitDeductionService)
        {
            var apiDeductions = await apiDeductionService.GetDeductionsForEmployeeAsync(employeeId);
            var benefitDeductions = benefitDeductionService.GetDeductionsForEmployee(employeeId);

            var ccss = contractType == "Servicios Profesionales" ? 0m : _ccssCalculator.CalculateDeduction(grossSalary);
            var tax = contractType == "Servicios Profesionales" ? 0m : _incomeTaxCalculator.Calculate(grossSalary);

            var totalDeductions = apiDeductions.Values.Sum()
                                 + benefitDeductions.Sum(d => d.DeductionValue)
                                 + ccss + tax;
            var netSalary = grossSalary - totalDeductions;

            return new PayrollSummary
            {
                EmployeeId = employeeId,
                ContractType = contractType,
                RegistersHours = registersHours,
                GrossSalary = grossSalary,
                NetSalary = netSalary,
                TotalDeductions = totalDeductions,
                ApiDeductions = apiDeductions,
                BenefitDeductions = benefitDeductions,
                CCSSDeduction = ccss,
                IncomeTax = tax
            };
        }
    }
}
