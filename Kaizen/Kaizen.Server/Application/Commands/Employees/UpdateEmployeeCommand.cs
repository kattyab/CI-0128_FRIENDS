using Kaizen.Server.Application.Dtos.Employees;
using MediatR;

namespace Kaizen.Server.Application.Commands.Employees
{
    public record UpdateEmployeeCommand(Guid EmployeeId, EmployeeDetailsDto EmployeeDto) : IRequest<bool>;
}
