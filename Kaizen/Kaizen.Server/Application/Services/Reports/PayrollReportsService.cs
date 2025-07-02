using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;
using System.Threading.Tasks;

namespace Kaizen.Server.Application.Services.Reports
{
    public class PayrollReportsService: IPayrollReportsService
    {
        private readonly IReportsRepository _reportsRepository;

        private const decimal RateSEM = 0.0925m;
        private const decimal RateIVM = 0.0542m;
        private const decimal RateCuotaPatronalBancoPopular = 0.0025m;
        private const decimal RateAsignacionesFamiliares = 0.0500m;
        private const decimal RateIMAS = 0.0050m;
        private const decimal RateINA = 0.0150m;
        private const decimal RateAporteBancoPopular = 0.0025m;
        private const decimal RateFCL = 0.0300m;
        private const decimal RateFondoPensionesComplementarias = 0.0050m;
        private const decimal RateINS = 0.0100m;

        private const decimal RateEmployeeSEM = 0.0550m;
        private const decimal RateEmployeeIVM = 0.0417m;
        private const decimal RateEmployeeBancoPopular = 0.0100m;

        public PayrollReportsService(IReportsRepository reportsRepository)
        {
            _reportsRepository = reportsRepository;
        }

        public async Task<IEnumerable<OwnerPayrollReport>> ExecuteAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Company ID cannot be empty", nameof(companyId));

            return await _reportsRepository.GetPayrollReportsByCompanyAsync(companyId);
        }

        public OwnerPayrollReport CalculateLaborCharges(OwnerPayrollReport report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            decimal totalSalarios = report.PorHorasAmount + report.TiempoCompletoAmount;
            CalculatePayrollDeductions(report, totalSalarios);

            return report;
        }

        private static void CalculatePayrollDeductions(OwnerPayrollReport report, decimal totalSalarios)
        {
            report.SEM = totalSalarios * RateSEM;
            report.IVM = totalSalarios * RateIVM;
            report.CuotaPatronalBancoPopular = totalSalarios * RateCuotaPatronalBancoPopular;
            report.AsignacionesFamiliares = totalSalarios * RateAsignacionesFamiliares;
            report.IMAS = totalSalarios * RateIMAS;
            report.INA = totalSalarios * RateINA;
            report.AporteBancoPopular = totalSalarios * RateAporteBancoPopular;
            report.FCL = totalSalarios * RateFCL;
            report.FondoPensionesComplementarias = totalSalarios * RateFondoPensionesComplementarias;
            report.INS = totalSalarios * RateINS;
        }

        public async Task<IEnumerable<EmployeePayrollReport>> ExecuteEmployeeAsync(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("Employee ID cannot be empty", nameof(employeeId));

            return await _reportsRepository.GetEmployeePayrollReportsByEmployeeAsync(employeeId);
        }

        public async Task<EmployeePayrollReport> CalculateOptionalDeductionsAsync(EmployeePayrollReport report)
        {
            var optionalDeductions = await _reportsRepository.GetOptionalDeductionsByPayrollAsync(report.PayrollID);
            report.OptionalDeductions = optionalDeductions.ToList();
            report.TotalOptionalDeductions = optionalDeductions.Sum(od => od.Amount);

            return report;
        }

        public EmployeePayrollReport CalculateObligatoryDeductions(EmployeePayrollReport report)
        {
            ArgumentNullException.ThrowIfNull(report);

            if (!IsServiciosProfesionales(report))
            {
                report.SEM = report.BruteSalary * RateEmployeeSEM;
                report.IVM = report.BruteSalary * RateEmployeeIVM;
                report.EmployeeAportacionBancoPopular = report.BruteSalary * RateEmployeeBancoPopular;

                report.TotalObligatoryDeductions = report.SEM + report.IVM +
                                                 report.EmployeeAportacionBancoPopular + report.IncomeTax;
            }

            return report;
        }
        private static bool IsServiciosProfesionales(EmployeePayrollReport report)
        {
            return report.ContractType == "Servicios Profesionales";
        }
    }
}
