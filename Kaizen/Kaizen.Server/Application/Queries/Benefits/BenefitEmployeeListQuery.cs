using MediatR;
using Kaizen.Server.Application.Dtos.Benefits;

namespace Kaizen.Server.Application.Queries.Benefits
{
    public class BenefitEmployeeListQuery : IRequest<List<BenefitEmployeeListDto>>
    {
        public string Email { get; set; }

        public BenefitEmployeeListQuery(string email)
        {
            Email = email;
        }
    }
}
