using Kaizen.Server.Application.Dtos.Reports;
using Kaizen.Server.Application.Interfaces.Reports;

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

            return report;
        }
    }
}
