using Kaizen.Server.Application.Commands.Benefits;
using Kaizen.Server.Application.Interfaces.Benefits;
using Microsoft.Data.SqlClient;
using System.Data;

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
                const string getEmployeeAndValidateQuery = @"
                    SELECT e.EmpId FROM Employees e
                    INNER JOIN Users u ON e.PersonPK = u.PersonPK
                    WHERE u.Email = @Email AND
                          e.IsDeleted = 0";

                Guid employeeId;
                using (var getEmployeeCommand = new SqlCommand(getEmployeeAndValidateQuery, connection, transaction))
                {
                    getEmployeeCommand.Parameters.AddWithValue("@Email", command.Email);
                    using var reader = await getEmployeeCommand.ExecuteReaderAsync();

                    if (!await reader.ReadAsync())
                        throw new InvalidOperationException("Employee not found with the provided email.");

                    var isDeleted = reader.GetBoolean("IsDeleted");
                    if (isDeleted)
                    {
                        return;
                    }

                    employeeId = reader.GetGuid("EmpId");
                }

                if (!string.IsNullOrEmpty(command.AssocName))
                {
                    const string insertAssocNameQuery = @"
                    IF EXISTS (SELECT 1 FROM EmployeeApiParameters WHERE ParameterKey = 'assocName' AND EmployeeId = @EmployeeId AND ApiConfigId = @ApiConfigId)
                        UPDATE EmployeeApiParameters 
                        SET ParameterValue = @ParameterValue
                        WHERE ParameterKey = 'assocName' AND EmployeeId = @EmployeeId AND ApiConfigId = @ApiConfigId
                    ELSE
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
                    IF EXISTS (SELECT 1 FROM EmployeeApiParameters WHERE ParameterKey = 'dependents' AND EmployeeId = @EmployeeId AND ApiConfigId = @ApiConfigId)
                        UPDATE EmployeeApiParameters 
                        SET ParameterValue = @ParameterValue
                        WHERE ParameterKey = 'dependents' AND EmployeeId = @EmployeeId AND ApiConfigId = @ApiConfigId
                    ELSE
                        INSERT INTO EmployeeApiParameters (EmployeeId, ApiConfigId, ParameterKey, ParameterValue)
                        VALUES (@EmployeeId, @ApiConfigId, 'assocName', @ParameterValue)";

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
