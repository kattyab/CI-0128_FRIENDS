using Kaizen.Server.Application.Dtos.BenefitDeductions;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionRepository
    {
        Task<List<Benefit>> GetBenefitsByCompanyAsync(Guid companyID, PayrollTransactionContext context = null);
    }
}
