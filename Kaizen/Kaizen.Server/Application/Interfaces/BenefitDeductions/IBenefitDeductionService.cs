using Kaizen.Server.Application.Dtos.BenefitDeductions;

namespace Kaizen.Server.Application.Interfaces.BenefitDeductions
{
    public interface IBenefitDeductionService
    {
        Task<List<BenefitDeductionResult>> GetBenefitDeductionsForEmployeeAsync(Guid employeeID);
    }
}
