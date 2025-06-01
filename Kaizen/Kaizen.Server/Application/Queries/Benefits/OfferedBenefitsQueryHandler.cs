using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using MediatR;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class OfferedBenefitsQueryHandler : IRequestHandler<OfferedBenefitsQuery, List<OfferedBenefitDto>>
    {
        private readonly IOfferedBenefitsRepository _repository;

        public OfferedBenefitsQueryHandler(IOfferedBenefitsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OfferedBenefitDto>> Handle(OfferedBenefitsQuery request, CancellationToken cancellationToken)
        {
            var benefits = await _repository.GetAvailableBenefitsForEmployee(request.Email);

            if (!request.IncludeUnavailable)
            {
                benefits = benefits.Where(b => b.IsAvailable).ToList();
            }

            return benefits;
        }
    }
}
