using Kaizen.Server.Application.Dtos.Employees;
using Kaizen.Server.Application.Interfaces.Employees;
using MediatR;

namespace Kaizen.Server.Application.Queries.Employees
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto?>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeDetailsDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(request.EmployeeId);
            return employee;
        }
    }
}
