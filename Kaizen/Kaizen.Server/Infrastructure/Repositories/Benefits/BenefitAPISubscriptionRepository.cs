using Microsoft.Data.SqlClient;
using Kaizen.Server.Application.Interfaces.Benefits;
using Kaizen.Server.Application.Commands.Benefits;

namespace Kaizen.Server.Infrastructure.Repositories.Benefits
{
    public class BenefitAPISubscriptionRepository : IBenefitAPISubscriptionRepository
    {
        private readonly string _connectionString;

        public BenefitAPISubscriptionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDB")
                ?? throw new ArgumentException("Connection string not found");
        }

        public async Task SubscribeAPIBenefitAsync(SubscribeBenefitAPICommand command)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string getEmployeeIdQuery = @"
                    SELECT e.EmpId FROM Employees e
                    INNER JOIN Users u ON e.PersonPK = u.PersonPK
                    WHERE u.Email = @Email";

                Guid employeeId;
                using (var getEmployeeCommand = new SqlCommand(getEmployeeIdQuery, connection, transaction))
                {
                    getEmployeeCommand.Parameters.AddWithValue("@Email", command.Email);
                    var result = await getEmployeeCommand.ExecuteScalarAsync();

                    if (result == null)
                        throw new InvalidOperationException("Employee not found with the provided email.");

                    employeeId = (Guid)result;
                }

                if (!string.IsNullOrEmpty(command.AssocName))
                {
                    const string insertAssocNameQuery = @"
                        INSERT INTO EmployeeApiParameters (EmployeeId, ApiConfigId, ParameterKey, ParameterValue)
                        VALUES (@EmployeeId, @ApiConfigId, 'assocName', @ParameterValue)";

                    using var assocNameCommand = new SqlCommand(insertAssocNameQuery, connection, transaction);
                    assocNameCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                    assocNameCommand.Parameters.AddWithValue("@ApiConfigId", command.Id);
                    assocNameCommand.Parameters.AddWithValue("@ParameterValue", command.AssocName);

                    await assocNameCommand.ExecuteNonQueryAsync();
                }

                if (!string.IsNullOrEmpty(command.Dependents))
                {
                    const string insertDependentsQuery = @"
                        INSERT INTO EmployeeApiParameters (EmployeeId, ApiConfigId, ParameterKey, ParameterValue)
                        VALUES (@EmployeeId, @ApiConfigId, 'dependents', @ParameterValue)";

                    using var dependentsCommand = new SqlCommand(insertDependentsQuery, connection, transaction);
                    dependentsCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                    dependentsCommand.Parameters.AddWithValue("@ApiConfigId", command.Id);
                    dependentsCommand.Parameters.AddWithValue("@ParameterValue", command.Dependents);

                    await dependentsCommand.ExecuteNonQueryAsync();
                }

                const string insertChosenApiQuery = @"
                    INSERT INTO ChosenAPIs (EmployeePK, ApiID)
                    VALUES (@EmployeePK, @ApiID)";

                using var chosenApiCommand = new SqlCommand(insertChosenApiQuery, connection, transaction);
                chosenApiCommand.Parameters.AddWithValue("@EmployeePK", employeeId);
                chosenApiCommand.Parameters.AddWithValue("@ApiID", command.Id);

                await chosenApiCommand.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
