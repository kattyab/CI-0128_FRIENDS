using Kaizen.Server.Application.Dtos.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using MediatR;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class EmployeeBenefitListQueryHandler : IRequestHandler<EmployeeBenefitListQuery, List<EmployeeBenefitListDto>>
    {
        private readonly IEmployeeBenefitListRepository _repository;

        public EmployeeBenefitListQueryHandler(IEmployeeBenefitListRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmployeeBenefitListDto>> Handle(EmployeeBenefitListQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetEmployeeBenefitList(request.Email);
        }
    }
}
