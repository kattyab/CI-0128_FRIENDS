using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Application.Interfaces.Services;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Services
{
    public class BenefitsService : IBenefitsService
    {
        private readonly IBenefitsRepository _benefitsRepository;
        private readonly string _connectionString;

        public BenefitsService(IBenefitsRepository benefitsRepository, IConfiguration configuration)
        {
            this._benefitsRepository = benefitsRepository;
            this._connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public void DeleteBenefit(Guid benefitId)
        {
            if (benefitId == Guid.Empty)
            {
                throw new ArgumentException("Benefit ID cannot be empty.", nameof(benefitId));
            }

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var isBenefitSubscribed = this._benefitsRepository.GetIfBenefitIsSubscribed(benefitId, connection, transaction);

                if (isBenefitSubscribed)
                {
                    var isBenefitOnPayroll = this._benefitsRepository.GetIfBenefitIsOnPayroll(benefitId, connection, transaction);

                    if (isBenefitOnPayroll)
                    {
                        this._benefitsRepository.SoftDeleteAndNotifyBenefit(benefitId, connection, transaction);
                    }
                    else
                    {
                        this._benefitsRepository.FullDeleteAndNotifyBenefit(benefitId, connection, transaction);
                    }
                }
                else
                {
                    this._benefitsRepository.DeleteBenefit(benefitId, connection, transaction);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("An error occurred while deleting the benefit.", ex);
            }
        }

        public void DeleteBenefit(int id, Guid companyPK)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than zero.", nameof(id));
            }
            if (companyPK == Guid.Empty)
            {
                throw new ArgumentException("Company PK cannot be empty.", nameof(companyPK));
            }

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                this._benefitsRepository.DeleteBenefit(id, companyPK, connection, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("An error occurred while deleting the benefit by ID.", ex);
            }
        }
    }
}
