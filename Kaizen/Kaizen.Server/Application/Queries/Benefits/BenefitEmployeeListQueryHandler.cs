using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using MediatR;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class BenefitEmployeeListQueryHandler : IRequestHandler<BenefitEmployeeListQuery, List<BenefitEmployeeListDto>>
    {
        private readonly IBenefitEmployeeListRepository _repository;

        public BenefitEmployeeListQueryHandler(IBenefitEmployeeListRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BenefitEmployeeListDto>> Handle(BenefitEmployeeListQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEmployeeBenefitList(request.Email);
        }
    }
}
