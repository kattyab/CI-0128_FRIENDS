namespace Kaizen.Server.Application.Interfaces.ApiDeductions;

public interface IApiDeductionServiceFactory
{
    IApiDeductionService Create(Guid companyId);
}
