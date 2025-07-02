using System;
using System.Data;
using Kaizen.Server.Application.Dtos;
using Kaizen.Server.Application.Interfaces.Repositories;
using Kaizen.Server.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace Kaizen.Server.Infrastructure.Repositories;

public class EmployeeDashboardRepository : IEmployeeDashboardRepository
{
    private readonly string _connectionString;

    public EmployeeDashboardRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KaizenDb")
            ?? throw new InvalidOperationException("Connection string 'KaizenDb' not found.");
    }

    public EmployeeDashboardDto GetDashboardByUserPK(Guid userPK)
    {
        try
        {
            var dashboard = GetEmployeeBasicInfoAndPayrolls(userPK);
            LoadOptionalDeductions(userPK, dashboard);
            return dashboard;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al obtener el dashboard del empleado.", ex);
        }
    }

    private EmployeeDashboardDto GetEmployeeBasicInfoAndPayrolls(Guid userPK)
    {
        var dashboard = new EmployeeDashboardDto();

        const string commandText = @"
        WITH UserInfo AS (
            SELECT u.UserPK, u.PersonPK, p.Name, p.LastName, e.JobPosition,
                   e.StartDate, e.BruteSalary, e.ContractType, e.EmpID
            FROM Users u
            JOIN Persons p ON u.PersonPK = p.PersonPK
            JOIN Employees e ON u.PersonPK = e.PersonPK
            WHERE u.UserPK = @UserPK
        ),
        RecentPayrolls AS (
            SELECT pr.PayrollID, pr.PaidTo AS EmpID, pr.GeneralPayrollPK,
                   pr.IncomeTax, pr.CCSS, pr.BrutePaid, pr.NetPaid,
                   gp.ExecutedOn,
                   ROW_NUMBER() OVER (ORDER BY gp.ExecutedOn DESC) AS rn
            FROM Payrolls pr
            JOIN GeneralPayrolls gp ON pr.GeneralPayrollPK = gp.GeneralPayrollsID
            WHERE pr.PaidTo = (SELECT EmpID FROM UserInfo)
        )
        SELECT ui.UserPK, ui.PersonPK, ui.Name, ui.LastName, ui.JobPosition,
               ui.StartDate, ui.BruteSalary, ui.ContractType, ui.EmpID,
               rp.PayrollID, rp.GeneralPayrollPK, rp.ExecutedOn,
               rp.IncomeTax, rp.CCSS, rp.BrutePaid, rp.NetPaid
        FROM UserInfo ui
        LEFT JOIN RecentPayrolls rp ON ui.EmpID = rp.EmpID AND rp.rn <= 3
        ORDER BY rp.ExecutedOn DESC;";

        SqlParameter[] parameters = [new("@UserPK", SqlDbType.UniqueIdentifier) { Value = userPK }];
        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text, parameters);

        while (reader.Read())
        {
            if (dashboard.UserPK == Guid.Empty)
            {
                MapEmployeeBasicInfo(reader, dashboard);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("PayrollID")))
            {
                dashboard.RecentPayrolls.Add(MapPayroll(reader));
            }
        }

        return dashboard;
    }

    private void LoadOptionalDeductions(Guid userPK, EmployeeDashboardDto dashboard)
    {
        const string optionalQuery = @"
        WITH UserInfo AS (
            SELECT e.EmpID
            FROM Users u
            JOIN Employees e ON u.PersonPK = e.PersonPK
            WHERE u.UserPK = @UserPK
        ),
        RecentPayroll AS (
            SELECT TOP 1 PayrollID
            FROM Payrolls
            WHERE PaidTo = (SELECT EmpID FROM UserInfo)
            ORDER BY PayrollID DESC
        )
        SELECT od.Name, od.Amount, od.PayrollID
        FROM OptionalDeductions od
        WHERE od.PayrollID = (SELECT PayrollID FROM RecentPayroll);";

        SqlParameter[] parameters = [new("@UserPK", SqlDbType.UniqueIdentifier) { Value = userPK }];
        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, optionalQuery, CommandType.Text, parameters);

        while (reader.Read())
        {
            if (!reader.IsDBNull(reader.GetOrdinal("Name")) && !reader.IsDBNull(reader.GetOrdinal("Amount")))
            {
                dashboard.OptionalDeductions.Add(new OptionalDeductionDto
                {
                    OptionalDeductionName = reader.GetString(reader.GetOrdinal("Name")),
                    OptionalDeductionAmount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    OptionalDeductionPayrollId = !reader.IsDBNull(reader.GetOrdinal("PayrollID"))
                        ? reader.GetGuid(reader.GetOrdinal("PayrollID"))
                        : Guid.Empty
                });
            }
        }
    }

    private void MapEmployeeBasicInfo(SqlDataReader reader, EmployeeDashboardDto dashboard)
    {
        dashboard.UserPK = GetGuid(reader, "UserPK");
        dashboard.PersonPK = GetGuid(reader, "PersonPK");
        dashboard.Name = GetString(reader, "Name");
        dashboard.LastName = GetString(reader, "LastName");
        dashboard.JobPosition = GetString(reader, "JobPosition");
        dashboard.StartDate = GetDateTime(reader, "StartDate");
        dashboard.BruteSalary = GetDecimal(reader, "BruteSalary");
        dashboard.ContractType = GetString(reader, "ContractType");
        dashboard.EmpID = GetGuid(reader, "EmpID");
    }

    private PayrollInfoDto MapPayroll(SqlDataReader reader)
    {
        return new PayrollInfoDto
        {
            PayrollID = GetGuid(reader, "PayrollID"),
            GeneralPayrollPK = GetGuid(reader, "GeneralPayrollPK"),
            ExecutedOn = GetDateTime(reader, "ExecutedOn"),
            IncomeTax = GetDecimal(reader, "IncomeTax"),
            CCSS = GetDecimal(reader, "CCSS"),
            BrutePaid = GetDecimal(reader, "BrutePaid"),
            NetPaid = GetDecimal(reader, "NetPaid")
        };
    }

    private Guid GetGuid(SqlDataReader reader, string column) =>
        !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetGuid(reader.GetOrdinal(column)) : Guid.Empty;

    private string GetString(SqlDataReader reader, string column) =>
        !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetString(reader.GetOrdinal(column)) : string.Empty;

    private DateTime GetDateTime(SqlDataReader reader, string column) =>
        !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetDateTime(reader.GetOrdinal(column)) : DateTime.MinValue;

    private decimal GetDecimal(SqlDataReader reader, string column) =>
        !reader.IsDBNull(reader.GetOrdinal(column)) ? reader.GetDecimal(reader.GetOrdinal(column)) : 0m;


}

