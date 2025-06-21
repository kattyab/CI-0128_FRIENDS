using Kaizen.Server.Application.Dtos.Reports;

namespace Kaizen.Server.Application.Interfaces.Reports
{
    public interface IPayrollReportsService
    {
        Task<IEnumerable<OwnerPayrollReport>> ExecuteAsync(Guid companyId);
        OwnerPayrollReport CalculateLaborCharges(OwnerPayrollReport report);
    }
}
