using Microsoft.Data.SqlClient;
using System.Data;
using Kaizen.Server.Application.Dtos;

namespace Kaizen.Server.Infrastructure.Repositories
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
                        e.StartDate,
                        e.BankAccount,
                        e.BruteSalary AS GrossSalary,
                        e.PayCycleType AS PayCycle,
                        e.RegistersHours,
                        u.Email,
                        u.Role,
                        u.Active AS Status,
                        pp.Number AS PhoneNumber,
                        adc.Name AS ApiName,
                        b.Name AS BenefitName
                    FROM 
                        dbo.Employees e
                        INNER JOIN dbo.Persons p ON e.PersonPK = p.PersonPK
                        LEFT JOIN dbo.Users u ON p.PersonPK = u.PersonPK
                        LEFT JOIN dbo.PersonPhoneNumbers pp ON p.PersonPK = pp.PersonPK
                        LEFT JOIN dbo.ChosenAPIs ca ON e.EmpID = ca.EmployeePK
                        LEFT JOIN dbo.ApiDeductionConfigs adc ON ca.ApiID = adc.Id
                        LEFT JOIN dbo.ChosenBenefits cb ON e.EmpID = cb.EmployeeID
                        LEFT JOIN dbo.Benefits b ON cb.BenefitID = b.ID
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
            var apiNames = new List<string>();
            var benefitNames = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                // Collect phone numbers
                var phoneNumber = dataRow["PhoneNumber"].ToString();
                if (!string.IsNullOrEmpty(phoneNumber) && !phoneNumbers.Contains(phoneNumber))
                {
                    phoneNumbers.Add(phoneNumber);
                }

                // Collect API names
                var apiName = dataRow["ApiName"].ToString();
                if (!string.IsNullOrEmpty(apiName) && !apiNames.Contains(apiName))
                {
                    apiNames.Add(apiName);
                }

                // Collect benefit names
                var benefitName = dataRow["BenefitName"].ToString();
                if (!string.IsNullOrEmpty(benefitName) && !benefitNames.Contains(benefitName))
                {
                    benefitNames.Add(benefitName);
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
                WorkHours = row["WorkHours"] != DBNull.Value ? Convert.ToInt32(row["WorkHours"]) : null,
                StartDate = Convert.ToDateTime(row["StartDate"]),
                BankAccount = row["BankAccount"].ToString(),
                GrossSalary = Convert.ToDecimal(row["GrossSalary"]),
                PayCycle = row["PayCycle"]?.ToString(),
                RegistersHours = Convert.ToBoolean(row["RegistersHours"]),
                Email = row["Email"]?.ToString(),
                Role = row["Role"]?.ToString(),
                Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]),
                PhoneNumbers = phoneNumbers,
                ChosenApiNames = apiNames,
                ChosenBenefitNames = benefitNames
            };
        }
    }
}