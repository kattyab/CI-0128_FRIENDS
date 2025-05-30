using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;

namespace Kaizen.Server.Application.Services.BenefitDeductions
{
    public class BenefitDeductionCalculator : IBenefitDeductionService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBenefitRepository _benefitRepository;

        public BenefitDeductionCalculator(IEmployeeRepository empRepo, IBenefitRepository benRepo)
        {
            _employeeRepository = empRepo;
            _benefitRepository = benRepo;
        }

        public List<BenefitDeductionResult> GetDeductionsForEmployee(Guid employeeID)
        {
            var employee = _employeeRepository.GetById(employeeID);
            var chosenIDs = _employeeRepository.GetChosenBenefitIDs(employeeID);
            var allBenefits = _benefitRepository.GetCompanyBenefits(employee.WorksFor);
            var selectedBenefits = allBenefits.Where(b => chosenIDs.Contains(b.BenefitID));

            var results = new List<BenefitDeductionResult>();

            foreach (var benefit in selectedBenefits)
            {
                if (!IsEligible(employee, benefit)) continue;

                decimal deduction = 0;
                if (benefit.IsFixed && benefit.FixedValue.HasValue)
                    deduction = benefit.FixedValue.Value;
                else if (benefit.IsPercetange && benefit.PercentageValue.HasValue)
                    deduction = Math.Round((benefit.PercentageValue.Value / 100m) * employee.BruteSalary, 2);

                results.Add(new BenefitDeductionResult
                {
                    BenefitName = benefit.Name,
                    DeductionValue = deduction
                });
            }

            return results;
        }

        private bool IsEligible(Employee emp, Benefit benefit) =>
            MatchesContractType(emp, benefit) && MeetsMinMonths(emp, benefit);

        private bool MatchesContractType(Employee emp, Benefit benefit) =>
            (benefit.IsFullTime && emp.ContractType == "Tiempo Completo") ||
            (benefit.IsPartTime && emp.ContractType == "Medio Tiempo") ||
            (benefit.IsByHours && emp.ContractType == "Por Horas") ||
            (benefit.IsByService && emp.ContractType == "Servicios Profesionales");

        private bool MeetsMinMonths(Employee emp, Benefit benefit)
        {
            var now = DateTime.Now;
            var months = ((now.Year - emp.StartDate.Year) * 12) + now.Month - emp.StartDate.Month;
            return months >= benefit.MinWorkDurationMonths;
        }
    }
}
