using Kaizen.Server.Application.Dtos.Employees;
using MediatR;

namespace Kaizen.Server.Application.Queries.Employees
{
    public record GetEmployeeByIdQuery(Guid EmployeeId) : IRequest<EmployeeDetailsDto?>;

}
