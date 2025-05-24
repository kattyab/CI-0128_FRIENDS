using Kaizen.Server.Application.Dtos.IncomeTax;
using MediatR;

namespace Kaizen.Server.Application.Queries.IncomeTax
{
    public class CalculateIncomeTax : IRequest<IncomeTaxDto>
    {
        public decimal MonthlyGrossSalary { get; set; }

        public CalculateIncomeTax(decimal monthlyGrossSalary)
        {
            MonthlyGrossSalary = monthlyGrossSalary;
        }
    }

}
