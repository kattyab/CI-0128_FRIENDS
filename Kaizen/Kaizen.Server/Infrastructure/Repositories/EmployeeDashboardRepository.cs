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

        try
        {
            using SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, commandText, CommandType.Text, parameters);
            while (reader.Read())
            {
                if (dashboard.UserPK == Guid.Empty)
                {
                    dashboard.UserPK = !reader.IsDBNull(reader.GetOrdinal("UserPK")) ? reader.GetGuid(reader.GetOrdinal("UserPK")) : Guid.Empty;
                    dashboard.PersonPK = !reader.IsDBNull(reader.GetOrdinal("PersonPK")) ? reader.GetGuid(reader.GetOrdinal("PersonPK")) : Guid.Empty;
                    dashboard.Name = !reader.IsDBNull(reader.GetOrdinal("Name")) ? reader.GetString(reader.GetOrdinal("Name")) : string.Empty;
                    dashboard.LastName = !reader.IsDBNull(reader.GetOrdinal("LastName")) ? reader.GetString(reader.GetOrdinal("LastName")) : string.Empty;
                    dashboard.JobPosition = !reader.IsDBNull(reader.GetOrdinal("JobPosition")) ? reader.GetString(reader.GetOrdinal("JobPosition")) : string.Empty;
                    dashboard.StartDate = !reader.IsDBNull(reader.GetOrdinal("StartDate")) ? reader.GetDateTime(reader.GetOrdinal("StartDate")) : DateTime.MinValue;
                    dashboard.BruteSalary = !reader.IsDBNull(reader.GetOrdinal("BruteSalary")) ? reader.GetDecimal(reader.GetOrdinal("BruteSalary")) : 0m;
                    dashboard.ContractType = !reader.IsDBNull(reader.GetOrdinal("ContractType")) ? reader.GetString(reader.GetOrdinal("ContractType")) : string.Empty;
                    dashboard.EmpID = !reader.IsDBNull(reader.GetOrdinal("EmpID")) ? reader.GetGuid(reader.GetOrdinal("EmpID")) : Guid.Empty;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("PayrollID")))
                {
                    var payroll = new PayrollInfoDto
                    {
                        PayrollID = reader.GetGuid(reader.GetOrdinal("PayrollID")),
                        GeneralPayrollPK = !reader.IsDBNull(reader.GetOrdinal("GeneralPayrollPK")) ? reader.GetGuid(reader.GetOrdinal("GeneralPayrollPK")) : Guid.Empty,
                        ExecutedOn = !reader.IsDBNull(reader.GetOrdinal("ExecutedOn")) ? reader.GetDateTime(reader.GetOrdinal("ExecutedOn")) : DateTime.MinValue,
                        IncomeTax = !reader.IsDBNull(reader.GetOrdinal("IncomeTax")) ? reader.GetDecimal(reader.GetOrdinal("IncomeTax")) : 0m,
                        CCSS = !reader.IsDBNull(reader.GetOrdinal("CCSS")) ? reader.GetDecimal(reader.GetOrdinal("CCSS")) : 0m,
                        BrutePaid = !reader.IsDBNull(reader.GetOrdinal("BrutePaid")) ? reader.GetDecimal(reader.GetOrdinal("BrutePaid")) : 0m,
                        NetPaid = !reader.IsDBNull(reader.GetOrdinal("NetPaid")) ? reader.GetDecimal(reader.GetOrdinal("NetPaid")) : 0m
                    };

                    dashboard.RecentPayrolls.Add(payroll);
                }
            }


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
                if (!reader2.IsDBNull(reader2.GetOrdinal("Name")) && !reader2.IsDBNull(reader2.GetOrdinal("Amount")))
                {
                    dashboard.OptionalDeductions.Add(new OptionalDeductionDto
                    {
                        OptionalDeductionName = reader2.GetString(reader2.GetOrdinal("Name")),
                        OptionalDeductionAmount = reader2.GetDecimal(reader2.GetOrdinal("Amount")),
                        OptionalDeductionPayrollId = !reader2.IsDBNull(reader2.GetOrdinal("PayrollID")) ? reader2.GetGuid(reader2.GetOrdinal("PayrollID")) : Guid.Empty
                    });
                }
            }
        }
        catch (Exception ex)
        {

            throw new ApplicationException("Error al obtener el dashboard del empleado.", ex);
        }

        return dashboard;
    }

}

