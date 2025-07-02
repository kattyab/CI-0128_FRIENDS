using Kaizen.Server.Application.Interfaces.Companies;
using Kaizen.Server.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Kaizen.Server.Infrastructure.Contexts;

namespace Kaizen.Server.Infrastructure.Services.Companies
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _companiesRepository;
        private readonly IConfiguration _configuration;

        public CompaniesService(ICompaniesRepository companiesRepository, IConfiguration configuration)
        {
            this._companiesRepository = companiesRepository;
            this._configuration = configuration;
        }

        public async Task DeleteCompanyAsync(Guid companyPK)
        {
            var connectionString = _configuration.GetConnectionString("KaizenDb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("The connection string 'KaizenDb' is not defined or is empty.");
            using var transactionContext = new TransactionContext(connectionString);

            await transactionContext.OpenAsync();

            try
            {
                var isTherePayroll = await _companiesRepository.IsTherePayrollAsync(companyPK, transactionContext);

                if (isTherePayroll)
                {
                    await _companiesRepository.SoftDeleteCompanyAsync(companyPK, transactionContext);
                }
                else
                {
                    await _companiesRepository.FullDeleteCompanyAsync(companyPK, transactionContext);
                }
                await transactionContext.CommitAsync();
            }
            catch (Exception) { 
                await transactionContext.RollbackAsync();
                return;
            }
        }
    }
}
