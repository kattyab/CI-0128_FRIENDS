using Kaizen.Server.Application.Interfaces.Employees;
using MediatR;

namespace Kaizen.Server.Application.Commands.Employees
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
    {
        private readonly IEmployeeRepository _repository;

        public UpdateEmployeeCommandHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Business logic validation
            if (request.EmployeeDto == null)
            {
                return false;
            }

            var result = await _repository.UpdateAsync(request.EmployeeId, request.EmployeeDto);
            return result;
        }
    }
}
