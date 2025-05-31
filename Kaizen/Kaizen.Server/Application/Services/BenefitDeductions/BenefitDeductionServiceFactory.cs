using Kaizen.Server.Application.Interfaces.BenefitDeductions;

namespace Kaizen.Server.Application.Services.BenefitDeductions
{
    public class BenefitDeductionServiceFactory : IBenefitDeductionServiceFactory
    {
        private readonly IBenefitDeductionRepository _benefitDeductionRepository;
        private readonly IEmployeeDeductionRepository _employeeDeductionRepository;

        public BenefitDeductionServiceFactory(
            IBenefitDeductionRepository benefitDeductionRepository,
            IEmployeeDeductionRepository employeeDeductionRepository)
        {
            _benefitDeductionRepository = benefitDeductionRepository;
            _employeeDeductionRepository = employeeDeductionRepository;
        }

        public BenefitDeductionService Create(Guid companyId)
        {
            return new BenefitDeductionService(companyId, _benefitDeductionRepository, _employeeDeductionRepository);
        }
    }
}
