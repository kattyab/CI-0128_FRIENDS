using Kaizen.Server.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kaizen.Server.Repository
{
    public class EmployeeDetailsRepository
    {
        private readonly string _connectionString;

        public EmployeeDetailsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb");
        }

        public async Task<EmployeeDetailsDto> ObtainEmployeeDataById(Guid empId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var adapter = new SqlDataAdapter())
            {
                var query = @"
                    SELECT 
                        e.EmpID,
                        p.Id,
                        p.Name AS FirstName,
                        p.LastName,
                        p.Sex,
                        p.BirthDate,
                        p.Province,
                        p.Canton,
                        p.OtherSigns,
                        e.JobPosition,
                        e.ContractType,
                        e.WorkHours,
                        e.ExtraHours,
                        e.StartDate,
                        e.BankAccount,
                        e.BruteSalary AS GrossSalary,
                        e.PayCycleType AS PayCycle,
                        u.Email,
                        u.Role,
                        u.Active AS Status,
                        pp.Number AS PhoneNumber
                    FROM 
                        dbo.Employees e
                        INNER JOIN dbo.Persons p ON e.PersonPK = p.PersonPK
                        LEFT JOIN dbo.Users u ON p.PersonPK = u.PersonPK
                        LEFT JOIN dbo.PersonPhoneNumbers pp ON p.PersonPK = pp.PersonPK
                    WHERE 
                        e.EmpID = @EmpID;";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmpID", empId);
                adapter.SelectCommand = command;

                var table = new DataTable();

                await connection.OpenAsync();
                adapter.Fill(table);

                if (table.Rows.Count == 0)
                    return null;

                return MapRowToDto(table);
            }
        }

        private EmployeeDetailsDto MapRowToDto(DataTable table)
        {
            var row = table.Rows[0];
            var phoneNumbers = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                var phoneNumber = dataRow["PhoneNumber"].ToString();
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    phoneNumbers.Add(phoneNumber);
                }
            }

            return new EmployeeDetailsDto
            {
                EmpID = row["EmpID"].ToString(),
                Id = row["Id"]?.ToString(),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Sex = row["Sex"].ToString(),
                BirthDate = Convert.ToDateTime(row["BirthDate"]),
                Province = row["Province"]?.ToString(),
                Canton = row["Canton"]?.ToString(),
                OtherSigns = row["OtherSigns"]?.ToString(),
                JobPosition = row["JobPosition"]?.ToString(),
                ContractType = row["ContractType"].ToString(),
                WorkHours = row["WorkHours"] != DBNull.Value ? Convert.ToInt32(row["WorkHours"]) : (int?)null,
                ExtraHours = row["ExtraHours"] != DBNull.Value ? Convert.ToInt32(row["ExtraHours"]) : (int?)null,
                StartDate = Convert.ToDateTime(row["StartDate"]),
                BankAccount = row["BankAccount"].ToString(),
                GrossSalary = Convert.ToDecimal(row["GrossSalary"]),
                PayCycle = row["PayCycle"]?.ToString(),
                Email = row["Email"]?.ToString(),
                Role = row["Role"]?.ToString(),
                Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]),
                PhoneNumbers = phoneNumbers
            };
        }
    }
}
