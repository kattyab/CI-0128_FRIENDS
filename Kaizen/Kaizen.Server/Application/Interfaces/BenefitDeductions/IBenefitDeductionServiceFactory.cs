using Kaizen.Server.Application.Services.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionServiceFactory
    {
        BenefitDeductionService Create(Guid companyId);
    }
}
