using Kaizen.Server.Application.Dtos.IncomeTax;
using Kaizen.Server.Application.Interfaces.IncomeTax;
using MediatR;

namespace Kaizen.Server.Application.Queries.IncomeTax
{
    public class CalculateIncomeTaxHandler : IRequestHandler<CalculateIncomeTax, IncomeTaxDto>
    {
        private readonly IIncomeTaxCalculator _calculator;

        public CalculateIncomeTaxHandler(IIncomeTaxCalculator calculator)
        {
            _calculator = calculator;
        }

        public Task<IncomeTaxDto> Handle(CalculateIncomeTax request, CancellationToken cancellationToken)
        {
            var tax = _calculator.Calculate(request.MonthlyGrossSalary);
            return Task.FromResult(new IncomeTaxDto
            {
                GrossSalary = request.MonthlyGrossSalary,
                TaxAmount = tax
            });
        }
    }
}
