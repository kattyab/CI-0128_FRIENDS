using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Dtos.Companies;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Infrastructure.Repositories;

public interface ICompaniesRepository
{
    List<CompanyDto> GetCompanies();
    CompanyDto? GetCompany(Guid companyPK);
    void UpdateCompany(Guid companyPK, CompanyEditDto companyEditDto);
    Task<bool> IsTherePayrollAsync(Guid companyPK, TransactionContext context);
    Task SoftDeleteCompanyAsync(Guid companyPK, TransactionContext context);
    Task FullDeleteCompanyAsync(Guid companyPK, TransactionContext context);
}