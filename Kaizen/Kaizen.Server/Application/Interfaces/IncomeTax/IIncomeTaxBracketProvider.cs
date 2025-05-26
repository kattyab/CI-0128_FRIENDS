using Kaizen.Server.Infrastructure.Helpers.IncomeTax;

public interface IIncomeTaxBracketProvider
{
    List<IncomeTaxBracket> GetBrackets();
}