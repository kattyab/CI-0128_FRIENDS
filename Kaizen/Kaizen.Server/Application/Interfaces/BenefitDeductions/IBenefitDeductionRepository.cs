using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionRepository
    {
        List<Benefit> GetNonApiBenefitsByCompany(Guid companyID);
    }
}
