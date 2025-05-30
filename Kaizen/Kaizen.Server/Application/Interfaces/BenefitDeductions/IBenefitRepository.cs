using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitRepository
    {
        List<Benefit> GetCompanyBenefits(Guid companyID);
    }
}
