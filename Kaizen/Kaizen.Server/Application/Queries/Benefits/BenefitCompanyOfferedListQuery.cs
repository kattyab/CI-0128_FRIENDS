using Kaizen.Server.Application.Dtos.Benefits;
using MediatR;

public class BenefitCompanyOfferedListQuery : IRequest<List<BenefitCompanyOfferedListDto>>
{
    public string Email { get; set; }
    public bool IncludeUnavailable { get; set; }

    public BenefitCompanyOfferedListQuery(string email, bool includeUnavailable = false)
    {
        Email = email;
        IncludeUnavailable = includeUnavailable;
    }
}
