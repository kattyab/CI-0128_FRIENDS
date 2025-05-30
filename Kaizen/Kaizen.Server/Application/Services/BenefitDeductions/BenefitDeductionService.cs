using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;

namespace Kaizen.Server.Application.Services.BenefitDeductions
{
    public class BenefitDeductionService : IBenefitDeductionService
    {
        private readonly Guid _companyID;
        private readonly IBenefitDeductionRepository _benefitRepo;
        private readonly IEmployeeDeductionRepository _employeeRepo;

        public BenefitDeductionService(
            Guid companyID,
            IBenefitDeductionRepository benefitRepo,
            IEmployeeDeductionRepository employeeRepo)
        {
            _companyID = companyID;
            _benefitRepo = benefitRepo;
            _employeeRepo = employeeRepo;
        }

        public List<BenefitDeductionResult> GetDeductionsForEmployee(Guid employeeID)
        {
            // Load data fresh inside the method (could be cached if needed)
            var companyBenefits = _benefitRepo.GetNonApiBenefitsByCompany(_companyID);
            var employeeData = _employeeRepo.GetEmployeesByCompany(_companyID);
            var employeeChosenBenefits = _employeeRepo.GetChosenBenefitsByCompany(_companyID);

            if (!employeeChosenBenefits.ContainsKey(employeeID) || !employeeData.ContainsKey(employeeID))
                return new List<BenefitDeductionResult>();

            var chosenBenefitIDs = employeeChosenBenefits[employeeID];
            var employee = employeeData[employeeID];

            return companyBenefits
                .Where(b => chosenBenefitIDs.Contains(b.BenefitID) && IsEligible(employee, b))
                .Select(b => new BenefitDeductionResult
                {
                    BenefitName = b.Name,
                    DeductionValue = CalculateDeduction(b, employee.BruteSalary)
                })
                .ToList();
        }

        private static decimal CalculateDeduction(Benefit benefit, decimal salary)
        {
            if (benefit.IsFixed && benefit.FixedValue.HasValue)
                return benefit.FixedValue.Value;

            if (benefit.IsPercetange && benefit.PercentageValue.HasValue)
                return Math.Round((benefit.PercentageValue.Value / 100m) * salary, 2);

            return 0;
        }

        private static bool IsEligible(Employee emp, Benefit benefit)
        {
            return MatchesContractType(emp, benefit) && MeetsMinMonths(emp, benefit);
        }

        private static bool MatchesContractType(Employee emp, Benefit benefit)
        {
            return
                (benefit.IsFullTime && emp.ContractType == "Tiempo Completo") ||
                (benefit.IsPartTime && emp.ContractType == "Medio Tiempo") ||
                (benefit.IsByHours && emp.ContractType == "Por Horas") ||
                (benefit.IsByService && emp.ContractType == "Servicios Profesionales");
        }

        private static bool MeetsMinMonths(Employee emp, Benefit benefit)
        {
            var now = DateTime.Now;
            var months = ((now.Year - emp.StartDate.Year) * 12) + now.Month - emp.StartDate.Month;
            return months >= benefit.MinWorkDurationMonths;
        }
    }
}
