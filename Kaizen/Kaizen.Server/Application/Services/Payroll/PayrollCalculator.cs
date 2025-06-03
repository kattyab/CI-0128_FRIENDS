using Kaizen.Server.API.Controllers;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.ApiDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.CCSS;
using Kaizen.Server.Application.Interfaces.IncomeTax;


namespace Kaizen.Server.Application.Services.Payroll
{
    public class PayrollCalculator
    {
        private const decimal BiweeklyFactor = 2m;
        private const int DaysInAMonth = 30;
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
    IBenefitDeductionService benefitDeductionService,
    PayrollRequest payrollInformation)
        {
            var isBiweekly = GetIsBiweekly(employee);
            var daysWorked = CalculateDaysWorked(employee, payrollInformation.Start, payrollInformation.End);

            //prototype
            if (daysWorked == 0)
            {
                return new PayrollSummary
                {
                    EmployeeId = employee.EmpID,
                    ContractType = employee.ContractType,
                    RegistersHours = employee.RegistersHours,
                    GrossSalary = 0,
                    NetSalary = 0,
                    TotalDeductions = 0,
                    ApiDeductions = new(),
                    BenefitDeductions = new(),
                    CCSSDeduction = 0,
                    IncomeTax = 0
                };
            }

            var totalDaysInPeriod = isBiweekly ? 15 : 30;
            var proportionalSalary = (employee.BruteSalary / totalDaysInPeriod) * daysWorked;
            var isFullPeriod = daysWorked == totalDaysInPeriod;
            var apiDeductions = await apiDeductionService.GetDeductionsForEmployeeAsync(employee.EmpID);
            var benefitDeductions = isFullPeriod
                ? await benefitDeductionService.GetBenefitDeductionsForEmployeeAsync(employee.EmpID)
                : await benefitDeductionService.GetBenefitDeductionsForEmployeeAsync(employee.EmpID, proportionalSalary);
            if (isBiweekly)
            {
                apiDeductions = AdjustApiDeductionsForBiweekly(apiDeductions, benefitDeductions);
            }
            decimal salaryForDeductions = daysWorked == totalDaysInPeriod
                ? (isBiweekly ? employee.BruteSalary * 2 : employee.BruteSalary)
                : (isBiweekly ? proportionalSalary * 2 : proportionalSalary);
            var ccssDeduction = GetCcssDeduction(employee, salaryForDeductions);
            var incomeTaxDeduction = GetIncomeTaxDeduction(employee, salaryForDeductions);
            if (isBiweekly)
            {
                AdjustBiweeklyObligatoryDeductions(ref ccssDeduction, ref incomeTaxDeduction);
            }
            var totalDeductions = GetTotalDeductions(apiDeductions, benefitDeductions, ccssDeduction, incomeTaxDeduction);
            var netSalary = GetNetSalary(employee, totalDeductions);
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

        private int CalculateDaysWorked(EmployeePayroll employee, DateTime payrollPeriodStart, DateTime payrollPeriodEnd)
        {
            var effectiveStartDate = employee.StartDate > payrollPeriodStart
                ? employee.StartDate
                : payrollPeriodStart;

            var effectiveEndDate = employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodEnd
                ? employee.FireDate.Value
                : payrollPeriodEnd;

            if (effectiveStartDate > payrollPeriodEnd ||
                (employee.FireDate.HasValue && employee.FireDate.Value < payrollPeriodStart))
            {
                return 0;
            }

            var daysWorked = (effectiveEndDate - effectiveStartDate).Days + 1;

            return Math.Max(0, Math.Min(daysWorked, DaysInAMonth));
        }

        private static decimal GetNetSalary(EmployeePayroll employee, decimal totalDeductions)
        {
            return employee.BruteSalary - totalDeductions;
        }

        private static Dictionary<string, decimal> AdjustApiDeductionsForBiweekly(Dictionary<string, decimal> apiDeductions,
            List<BenefitDeductionResult> benefitDeductions)
        {
            apiDeductions = apiDeductions.ToDictionary(
                                            deductionEntry => deductionEntry.Key,
                                            deductionEntry => deductionEntry.Value / BiweeklyFactor
                                        );

            foreach (var benefitDeduction in benefitDeductions)
            {
                benefitDeduction.DeductionValue /= BiweeklyFactor;
            }

            return apiDeductions;
        }

        private static void AdjustBiweeklyObligatoryDeductions(ref decimal ccssDeduction, ref decimal incomeTaxDeduction)
        {
            ccssDeduction /= BiweeklyFactor;
            incomeTaxDeduction /= BiweeklyFactor;
        }

        private static bool GetIsBiweekly(EmployeePayroll employee)
        {
            return employee.PayrollTypeDescription.Equals("Biweekly", StringComparison.OrdinalIgnoreCase);
        }

        private static decimal GetTotalDeductions(Dictionary<string, decimal> apiDeductions, List<BenefitDeductionResult> benefitDeductions,
            decimal ccssDeduction, decimal incomeTaxDeduction)
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

    }
}
