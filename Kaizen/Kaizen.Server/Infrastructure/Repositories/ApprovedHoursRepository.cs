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
}
