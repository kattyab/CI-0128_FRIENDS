namespace Kaizen.Server.Application.Interfaces.IncomeTax
{
    public interface IIncomeTaxCalculator
    {
        decimal Calculate(decimal grossSalary);
    }
}
