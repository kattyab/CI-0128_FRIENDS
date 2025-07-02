using Kaizen.Server.Application.Dtos.OwnerDashboard;
using Kaizen.Server.Application.Interfaces.Services.OwnerDashboard;
using Kaizen.Server.Infrastructure.Repositories;

namespace Kaizen.Server.Application.Services.OwnerDashboard
{
    public class OwnerDashboardService : IOwnerDashboardService
    {
        private readonly OwnerDashboardRepository _repository;

        public OwnerDashboardService(OwnerDashboardRepository repository)
        {
            _repository = repository;
        }

        public Task<List<ContractCountResultDto>> GetContractCountsLast3MonthsAsync(Guid companyPk)
            => _repository.GetContractCountsLast3MonthsAsync(companyPk);

        public Task<List<LastPayrollDto>> GetLast3PayrollsAsync(Guid companyPk)
            => _repository.GetLast3PayrollsAsync(companyPk);

        public Task<PayrollCostBreakdownDto?> GetPayrollCostBreakdownAsync(Guid companyPk)
            => _repository.GetPayrollCostBreakdownAsync(companyPk);
    }
}
