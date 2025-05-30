using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionService
    {
        List<DeductionResult> GetDeductionsForEmployee(Guid employeeID);
    }
}
