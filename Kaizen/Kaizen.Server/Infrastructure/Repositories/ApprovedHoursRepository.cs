using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

// This class handles data insertion for the ApprovedHours table
public class ApprovedHoursRepository(IConfiguration configuration)
{
    // Retrieves the connection string from appsettings.json
    private readonly string _connectionString =
        configuration.GetConnectionString("KaizenDb")
        ?? throw new InvalidOperationException(
               "The connection string 'KaizenDb' is not defined in appsettings.json.");

    // Inserts a new record into the ApprovedHours table
    public void InsertApprovedHour(ApprovedHoursDto dto)
    {
        const string commandText = @"
        INSERT INTO ApprovedHours (
            ApprovalID,
            EmpID,
            StartDate,
            EndDate,
            HoursWorked,
            Status,
            IsSentForApproval,
            SupID
        )
        VALUES (
            NEWID(),
            @EmpID,
            @StartDate,
            @EndDate,
            @HoursWorked,
            NULL,
            @IsSentForApproval,
            NULL
        );";

        // Prepare the parameters
        SqlParameter[] parameters = [
            new("@EmpID", SqlDbType.UniqueIdentifier) { Value = dto.EmpID },
            new("@StartDate", SqlDbType.Date) { Value = dto.StartDate },
            new("@EndDate", SqlDbType.Date) { Value = dto.EndDate },
            new("@HoursWorked", SqlDbType.Decimal) { Value = dto.HoursWorked },
            new("@IsSentForApproval", SqlDbType.Bit) { Value = dto.IsSentForApproval }
        ];

        // Execute the insert command using SqlHelper
        SqlHelper.ExecuteNonQuery(_connectionString, commandText, CommandType.Text, parameters);
    }
    public List<ApprovedHoursDto> GetApprovedHoursByEmpId(Guid empId)
    {
        const string commandText = @"
        SELECT 
            ApprovalID,
            EmpID,
            SupID,
            StartDate,
            EndDate,
            HoursWorked,
            Status,
            IsSentForApproval
        FROM ApprovedHours
        WHERE EmpID = @EmpID;";

        var result = new List<ApprovedHoursDto>();

        SqlParameter[] parameters = [
            new("@EmpID", SqlDbType.UniqueIdentifier) { Value = empId }
        ];

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text, parameters);

        while (reader.Read())
        {
            result.Add(new ApprovedHoursDto
            {
                ApprovalID = reader.GetGuid(reader.GetOrdinal("ApprovalID")),
                EmpID = reader.GetGuid(reader.GetOrdinal("EmpID")),
                SupID = reader.IsDBNull(reader.GetOrdinal("SupID")) ? null : reader.GetGuid(reader.GetOrdinal("SupID")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                HoursWorked = reader.GetDecimal(reader.GetOrdinal("HoursWorked")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                IsSentForApproval = reader.GetBoolean(reader.GetOrdinal("IsSentForApproval"))
            });
        }

        return result;
    }
    public async Task<bool> UpdateStatusAndSentAsync(Guid approvalID, string status, bool isSentForApproval)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"UPDATE ApprovedHours
                  SET Status = @Status, IsSentForApproval = @IsSent
                  WHERE ApprovalID = @ApprovalID";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@IsSent", isSentForApproval);
        command.Parameters.AddWithValue("@ApprovalID", approvalID);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }




}
