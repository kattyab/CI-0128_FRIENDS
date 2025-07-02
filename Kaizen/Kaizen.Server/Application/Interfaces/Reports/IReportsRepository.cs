using Kaizen.Server.Application.Dtos.Reports;

namespace Kaizen.Server.Application.Interfaces.Reports
{
    public interface IReportsRepository
    {
        Task<IEnumerable<OwnerPayrollReport>> GetPayrollReportsByCompanyAsync(Guid companyId);
        Task<IEnumerable<EmployeePayrollReport>> GetEmployeePayrollReportsByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<OptionalDeduction>> GetOptionalDeductionsByPayrollAsync(Guid payrollId);
    }
}
