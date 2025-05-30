using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionRepository
    {
        List<Benefit> GetBenefitsByCompany(Guid companyID);
    }
}
