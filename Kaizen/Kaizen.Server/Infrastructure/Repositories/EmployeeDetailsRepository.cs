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

        public async Task<bool> UpdateEmployeeById(Guid empId, EmployeeDetailsDto employeeDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First, get the PersonPK from the Employee
                        var getPersonPKQuery = @"
                    SELECT PersonPK 
                    FROM dbo.Employees 
                    WHERE EmpID = @EmpID";

                        var getPersonPKCommand = new SqlCommand(getPersonPKQuery, connection, transaction);
                        getPersonPKCommand.Parameters.AddWithValue("@EmpID", empId);

                        var personPK = await getPersonPKCommand.ExecuteScalarAsync();
                        if (personPK == null)
                        {
                            transaction.Rollback();
                            return false; // Employee not found
                        }

                        // Update Persons table
                        var updatePersonQuery = @"
                    UPDATE dbo.Persons 
                    SET 
                        Id = @Id,
                        Name = @FirstName,
                        LastName = @LastName,
                        Sex = @Sex,
                        Province = @Province,
                        Canton = @Canton,
                        OtherSigns = @OtherSigns
                    WHERE PersonPK = @PersonPK";

                        var updatePersonCommand = new SqlCommand(updatePersonQuery, connection, transaction);
                        updatePersonCommand.Parameters.AddWithValue("@Id", employeeDto.Id ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@FirstName", employeeDto.FirstName ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@LastName", employeeDto.LastName ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@Sex", employeeDto.Sex ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@Province", employeeDto.Province ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@Canton", employeeDto.Canton ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@OtherSigns", employeeDto.OtherSigns ?? (object)DBNull.Value);
                        updatePersonCommand.Parameters.AddWithValue("@PersonPK", personPK);

                        await updatePersonCommand.ExecuteNonQueryAsync();

                        // Update Employees table
                        var updateEmployeeQuery = @"
                    UPDATE dbo.Employees 
                    SET 
                        JobPosition = @JobPosition,
                        ContractType = @ContractType,
                        WorkHours = @WorkHours,
                        BankAccount = @BankAccount,
                        BruteSalary = @GrossSalary,
                        PayCycleType = @PayCycle,
                        RegistersHours = @RegistersHours
                    WHERE EmpID = @EmpID";

                        var updateEmployeeCommand = new SqlCommand(updateEmployeeQuery, connection, transaction);
                        updateEmployeeCommand.Parameters.AddWithValue("@JobPosition", employeeDto.JobPosition ?? (object)DBNull.Value);
                        updateEmployeeCommand.Parameters.AddWithValue("@ContractType", employeeDto.ContractType ?? (object)DBNull.Value);
                        updateEmployeeCommand.Parameters.AddWithValue("@WorkHours", employeeDto.WorkHours ?? (object)DBNull.Value);
                        updateEmployeeCommand.Parameters.AddWithValue("@BankAccount", employeeDto.BankAccount ?? (object)DBNull.Value);
                        updateEmployeeCommand.Parameters.AddWithValue("@GrossSalary", employeeDto.GrossSalary);
                        updateEmployeeCommand.Parameters.AddWithValue("@PayCycle", employeeDto.PayCycle ?? (object)DBNull.Value);
                        updateEmployeeCommand.Parameters.AddWithValue("@RegistersHours", employeeDto.RegistersHours);
                        updateEmployeeCommand.Parameters.AddWithValue("@EmpID", empId);

                        await updateEmployeeCommand.ExecuteNonQueryAsync();

                        // Update Users table (if user exists)
                        var updateUserQuery = @"
                    UPDATE dbo.Users 
                    SET 
                        Email = @Email,
                        Role = @Role,
                        Active = @Status
                    WHERE PersonPK = @PersonPK";

                        var updateUserCommand = new SqlCommand(updateUserQuery, connection, transaction);
                        updateUserCommand.Parameters.AddWithValue("@Email", employeeDto.Email ?? (object)DBNull.Value);
                        updateUserCommand.Parameters.AddWithValue("@Role", employeeDto.Role ?? (object)DBNull.Value);
                        updateUserCommand.Parameters.AddWithValue("@Status", employeeDto.Status);
                        updateUserCommand.Parameters.AddWithValue("@PersonPK", personPK);

                        await updateUserCommand.ExecuteNonQueryAsync();

                        // Handle Phone Numbers - Delete existing and insert new ones
                        if (employeeDto.PhoneNumbers != null && employeeDto.PhoneNumbers.Any())
                        {
                            // Delete existing phone numbers
                            var deletePhoneQuery = @"
                        DELETE FROM dbo.PersonPhoneNumbers 
                        WHERE PersonPK = @PersonPK";

                            var deletePhoneCommand = new SqlCommand(deletePhoneQuery, connection, transaction);
                            deletePhoneCommand.Parameters.AddWithValue("@PersonPK", personPK);
                            await deletePhoneCommand.ExecuteNonQueryAsync();

                            // Insert new phone numbers
                            var insertPhoneQuery = @"
                        INSERT INTO dbo.PersonPhoneNumbers (PersonPK, Number) 
                        VALUES (@PersonPK, @Number)";

                            foreach (var phoneNumber in employeeDto.PhoneNumbers)
                            {
                                if (!string.IsNullOrWhiteSpace(phoneNumber))
                                {
                                    var insertPhoneCommand = new SqlCommand(insertPhoneQuery, connection, transaction);
                                    insertPhoneCommand.Parameters.AddWithValue("@PersonPK", personPK);
                                    insertPhoneCommand.Parameters.AddWithValue("@Number", phoneNumber.Trim());
                                    await insertPhoneCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of error
                        transaction.Rollback();

                        // Log the exception (you might want to use your logging framework here)
                        Console.WriteLine($"Error updating employee: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}