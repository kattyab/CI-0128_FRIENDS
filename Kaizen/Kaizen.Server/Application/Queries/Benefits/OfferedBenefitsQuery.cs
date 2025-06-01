using Kaizen.Server.Application.Dtos.Benefits;
using MediatR;

public class OfferedBenefitsQuery : IRequest<List<OfferedBenefitDto>>
{
    public string Email { get; set; }
    public bool IncludeUnavailable { get; set; }

    public OfferedBenefitsQuery(string email, bool includeUnavailable = false)
    {
        Email = email;
        IncludeUnavailable = includeUnavailable;
    }
}
