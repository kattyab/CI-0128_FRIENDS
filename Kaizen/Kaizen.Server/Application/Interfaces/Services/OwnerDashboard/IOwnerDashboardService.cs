using Kaizen.Server.Application.Dtos.OwnerDashboard;

namespace Kaizen.Server.Application.Interfaces.Services.OwnerDashboard
{
    public interface IOwnerDashboardService
    {
        Task<List<ContractCountResultDto>> GetContractCountsLast3MonthsAsync(Guid companyPk);
        Task<List<LastPayrollDto>> GetLast3PayrollsAsync(Guid companyPk);
        Task<PayrollCostBreakdownDto?> GetPayrollCostBreakdownAsync(Guid companyPk);
    }
}
