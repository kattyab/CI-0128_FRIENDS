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
        var dashboard = new EmployeeDashboardDto();

        const string commandText = @"
            WITH UserInfo AS (
                SELECT 
                    u.UserPK,
                    u.PersonPK,
                    p.Name,
                    p.LastName,
                    e.JobPosition,
                    e.StartDate,
                    e.BruteSalary,
                    e.ContractType,
                    e.EmpID
                FROM Users u
                JOIN Persons p ON u.PersonPK = p.PersonPK
                JOIN Employees e ON u.PersonPK = e.PersonPK
                WHERE u.UserPK = @UserPK
            ),
            RecentPayrolls AS (
                SELECT 
                    pr.PayrollID,
                    pr.PaidTo AS EmpID,
                    pr.GeneralPayrollPK,
                    pr.IncomeTax,
                    pr.CCSS,
                    pr.BrutePaid,
                    pr.NetPaid,
                    gp.ExecutedOn,
                    ROW_NUMBER() OVER (ORDER BY gp.ExecutedOn DESC) AS rn
                FROM Payrolls pr
                JOIN GeneralPayrolls gp ON pr.GeneralPayrollPK = gp.GeneralPayrollsID
                WHERE pr.PaidTo = (SELECT EmpID FROM UserInfo)
            )
            SELECT 
                ui.UserPK,
                ui.PersonPK,
                ui.Name,
                ui.LastName,
                ui.JobPosition,
                ui.StartDate,
                ui.BruteSalary,
                ui.ContractType,
                ui.EmpID,
                rp.PayrollID,
                rp.GeneralPayrollPK,
                rp.ExecutedOn,
                rp.IncomeTax,
                rp.CCSS,
                rp.BrutePaid,
                rp.NetPaid
            FROM UserInfo ui
            LEFT JOIN RecentPayrolls rp ON ui.EmpID = rp.EmpID AND rp.rn <= 3
            ORDER BY rp.ExecutedOn DESC;";

        SqlParameter[] parameters = [new("@UserPK", SqlDbType.UniqueIdentifier) { Value = userPK }];

        using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text, parameters);
        while (reader.Read())
        {
            if (dashboard.UserPK == Guid.Empty)
            {
                dashboard.UserPK = reader.GetGuid(reader.GetOrdinal("UserPK"));
                dashboard.PersonPK = reader.GetGuid(reader.GetOrdinal("PersonPK"));
                dashboard.Name = reader.GetString(reader.GetOrdinal("Name"));
                dashboard.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                dashboard.JobPosition = reader.GetString(reader.GetOrdinal("JobPosition"));
                dashboard.StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                dashboard.BruteSalary = reader.GetDecimal(reader.GetOrdinal("BruteSalary"));
                dashboard.ContractType = reader.GetString(reader.GetOrdinal("ContractType"));
                dashboard.EmpID = reader.GetGuid(reader.GetOrdinal("EmpID"));
            }

            if (!reader.IsDBNull(reader.GetOrdinal("PayrollID")))
            {
                dashboard.RecentPayrolls.Add(new PayrollInfoDto
                {
                    PayrollID = reader.GetGuid(reader.GetOrdinal("PayrollID")),
                    GeneralPayrollPK = reader.GetGuid(reader.GetOrdinal("GeneralPayrollPK")),
                    ExecutedOn = reader.GetDateTime(reader.GetOrdinal("ExecutedOn")),
                    IncomeTax = reader.GetDecimal(reader.GetOrdinal("IncomeTax")),
                    CCSS = reader.GetDecimal(reader.GetOrdinal("CCSS")),
                    BrutePaid = reader.GetDecimal(reader.GetOrdinal("BrutePaid")),
                    NetPaid = reader.GetDecimal(reader.GetOrdinal("NetPaid")),
                });
            }
        }

        // Segundo query: deducciones opcionales
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
            SELECT 
                od.Name,
                od.Amount,
                od.PayrollID
            FROM OptionalDeductions od
            WHERE od.PayrollID = (SELECT PayrollID FROM RecentPayroll);";

        SqlParameter[] parameters2 = [new("@UserPK", SqlDbType.UniqueIdentifier) { Value = userPK }];
        using SqlDataReader reader2 = SqlHelper.ExecuteReader(_connectionString, optionalQuery, CommandType.Text, parameters2);
        while (reader2.Read())
        {
            dashboard.OptionalDeductions.Add(new OptionalDeductionDto
            {
                OptionalDeductionName = reader2.GetString(reader2.GetOrdinal("Name")),
                OptionalDeductionAmount = reader2.GetDecimal(reader2.GetOrdinal("Amount")),
                OptionalDeductionPayrollId = reader2.GetGuid(reader2.GetOrdinal("PayrollID"))
            });
        }

        return dashboard;
    }
}
