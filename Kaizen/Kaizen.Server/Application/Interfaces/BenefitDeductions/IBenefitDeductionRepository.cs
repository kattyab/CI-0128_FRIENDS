using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionRepository
    {
        Task<List<Benefit>> GetBenefitsByCompanyAsync(Guid companyID);
    }
}
