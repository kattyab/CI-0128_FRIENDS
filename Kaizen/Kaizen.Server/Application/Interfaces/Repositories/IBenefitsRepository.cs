using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Interfaces.Repositories;

public interface IBenefitsRepository
{
    List<BenefitDto> GetBenefits(Guid companyPK);
    BenefitDto? GetBenefit(Guid guid, Guid companyPK);
    void UpdateBenefit(BenefitDto benefit, Guid companyPK);
}
