using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using MediatR;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class BenefitCompanyOfferedListQueryHandler : IRequestHandler<BenefitCompanyOfferedListQuery, List<BenefitCompanyOfferedListDto>>
    {
        private readonly IBenefitCompanyOfferedListRepository _repository;

        public BenefitCompanyOfferedListQueryHandler(IBenefitCompanyOfferedListRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BenefitCompanyOfferedListDto>> Handle(BenefitCompanyOfferedListQuery request, CancellationToken cancellationToken)
        {
            var benefits = await _repository.GetAvailableBenefitsForEmployee(request.Email);

            if (!request.IncludeUnavailable)
            {
                benefits = benefits.ToList();
            }

            return benefits;
        }
    }
}
