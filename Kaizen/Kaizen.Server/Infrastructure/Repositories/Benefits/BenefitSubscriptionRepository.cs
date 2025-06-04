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
            using var command = new SqlCommand("sp_InsertChosenBenefit", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@BenefitId", benefitId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
