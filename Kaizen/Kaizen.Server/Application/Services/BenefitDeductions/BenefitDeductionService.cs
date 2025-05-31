using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Application.Interfaces.BenefitDeductions;

namespace Kaizen.Server.Application.Services.BenefitDeductions
{
    public class BenefitDeductionService : IBenefitDeductionService
    {
        private readonly Guid _companyID;
        private readonly IBenefitDeductionRepository _benefitRepo;
        private readonly IEmployeeDeductionRepository _employeeRepo;

        private List<Benefit>? _companyBenefits;
        private Dictionary<Guid, EmployeeDto>? _employeeData;
        private Dictionary<Guid, List<Guid>>? _employeeChosenBenefits;

        private static readonly decimal _percentageDivider = 100m;
        private static readonly int _decimalPlacesToRound = 2;
        private static readonly int _monthsInYear = 12;

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
            if (_companyBenefits == null)
                _companyBenefits = _benefitRepo.GetBenefitsByCompany(_companyID);

            if (_employeeData == null)
                _employeeData = _employeeRepo.GetEmployeesByCompany(_companyID);

            if (_employeeChosenBenefits == null)
                _employeeChosenBenefits = _employeeRepo.GetChosenBenefitsByCompany(_companyID);

            if (!_employeeChosenBenefits.ContainsKey(employeeID) || !_employeeData.ContainsKey(employeeID))
                return new List<BenefitDeductionResult>();

            var chosenBenefitIDs = _employeeChosenBenefits[employeeID];
            var employee = _employeeData[employeeID];

            return _companyBenefits
                .Where(benefit => chosenBenefitIDs.Contains(benefit.BenefitID) && MeetsMinMonths(employee, benefit))
                .Select(benefit => new BenefitDeductionResult
                {
                    BenefitName = benefit.Name,
                    DeductionValue = CalculateDeduction(benefit, employee.BruteSalary)
                })
                .ToList();
        }

        private static decimal CalculateDeduction(Benefit benefit, decimal salary)
        {
            if (benefit.IsFixed && benefit.FixedValue.HasValue)
                return benefit.FixedValue.Value;

            if (benefit.IsPercetange && benefit.PercentageValue.HasValue)
                return Math.Round((benefit.PercentageValue.Value / _percentageDivider) * salary, _decimalPlacesToRound);


            return 0;
        }
        private static bool MeetsMinMonths(EmployeeDto emp, Benefit benefit)
        {
            var now = DateTime.Now;
            var months = ((now.Year - emp.StartDate.Year) * _monthsInYear) + now.Month - emp.StartDate.Month;
            return months >= benefit.MinWorkDurationMonths;
        }
    }
}
