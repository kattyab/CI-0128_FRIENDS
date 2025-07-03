using Kaizen.Server.Application.Interfaces.Benefits;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Benefits
{
    public class BenefitSubscriptionRepository : IBenefitSubscriptionRepository
    {
        private readonly string _connectionString;

        public BenefitSubscriptionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDB")!;
        }

        public async Task SubscribeAsync(string email, Guid benefitId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string validationQuery = @"
        SELECT c.IsDeleted 
        FROM Benefits b
        INNER JOIN Companies c ON b.OfferedBy = c.CompanyPK
        WHERE b.ID = @BenefitId";

            using var validationCommand = new SqlCommand(validationQuery, connection);
            validationCommand.Parameters.AddWithValue("@BenefitId", benefitId);

            var isDeleted = await validationCommand.ExecuteScalarAsync() as bool?;

            if (isDeleted == false)
            {
                using var command = new SqlCommand("sp_InsertChosenBenefit", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@BenefitId", benefitId);


                await command.ExecuteNonQueryAsync();
            }  
        }
    }
}
