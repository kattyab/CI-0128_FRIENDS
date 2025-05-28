using MediatR;
using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class EmployeeBenefitListQuery : IRequest<List<EmployeeBenefitListDto>>
    {
        public string Email { get; set; }

        public EmployeeBenefitListQuery(string email)
        {
            Email = email;
        }
    }
}
