using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Infrastructure.Repositories
{
  public interface IBenefitsRepository
  {
    BenefitDto? GetBenefit(Guid guid, Guid companyPK);
    void UpdateBenefit(BenefitDto benefit, Guid companyPK);
  }
}
