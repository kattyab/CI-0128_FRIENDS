using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class ApprovedHoursRepository : IApprovedHoursRepository
{
    private readonly string _connectionString;

    public ApprovedHoursRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException(
                "The connection string 'KaizenDb' is not defined in appsettings.json.");
    }

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

        SqlParameter[] parameters = [
            new("@EmpID", SqlDbType.UniqueIdentifier) { Value = dto.EmpID },
            new("@StartDate", SqlDbType.Date) { Value = dto.StartDate },
            new("@EndDate", SqlDbType.Date) { Value = dto.EndDate },
            new("@HoursWorked", SqlDbType.Decimal) { Value = dto.HoursWorked },
            new("@IsSentForApproval", SqlDbType.Bit) { Value = dto.IsSentForApproval }
        ];

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

    public List<ApprovedHoursDto> GetAllApprovedHours()
    {
        const string query = @"
            SELECT
                ah.ApprovalID,
                ah.StartDate,
                ah.EndDate,
                ah.Status,
                ah.HoursWorked,
                ah.IsSentForApproval,

                p.Name,
                p.LastName,

                e.StartDate AS EmployeeStartDate,
                e.ContractType,

                c.PayrollType
            FROM ApprovedHours ah
            JOIN Employees e ON ah.EmpID = e.EmpID
            JOIN Persons p ON e.PersonPK = p.PersonPK
            JOIN Companies c ON e.WorksFor = c.CompanyPK";

        var result = new List<ApprovedHoursDto>();

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, query, CommandType.Text);
        while (reader.Read())
        {
            result.Add(new ApprovedHoursDto
            {
                ApprovalID = reader.GetGuid(reader.GetOrdinal("ApprovalID")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                HoursWorked = reader.GetDecimal(reader.GetOrdinal("HoursWorked")),
                IsSentForApproval = reader.GetBoolean(reader.GetOrdinal("IsSentForApproval")),

                Name = reader.GetString(reader.GetOrdinal("Name")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),

                EmployeeStartDate = reader.GetDateTime(reader.GetOrdinal("EmployeeStartDate")),
                ContractType = reader.GetString(reader.GetOrdinal("ContractType")),
                PayrollType = reader.IsDBNull(reader.GetOrdinal("PayrollType")) ? null : reader.GetString(reader.GetOrdinal("PayrollType")),
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

    public async Task<bool> UpdateStatusAsync(Guid approvalID, string status)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"UPDATE ApprovedHours
                      SET Status = @Status
                      WHERE ApprovalID = @ApprovalID";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@ApprovalID", approvalID);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
