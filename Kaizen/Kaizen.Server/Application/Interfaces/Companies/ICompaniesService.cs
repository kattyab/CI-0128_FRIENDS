namespace Kaizen.Server.Application.Interfaces.Companies
{
    public interface ICompaniesService
    {
        public Task DeleteCompanyAsync(Guid companyId);
    }
}
