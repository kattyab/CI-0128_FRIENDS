using Kaizen.Server.Application.Dtos.Employees;
using Kaizen.Server.Application.Interfaces.Employees;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kaizen.Server.Infrastructure.Repositories.Employees
{
    public class EmployeeDetailsRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeDetailsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'KaizenDb' not found");
        }

        public async Task<EmployeeDetailsDto?> GetByIdAsync(Guid empId)
        {
            const string query = @"
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
                    e.EmpID = @EmpID";

            using var connection = new SqlConnection(_connectionString);
            using var adapter = new SqlDataAdapter();

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmpID", empId);
            adapter.SelectCommand = command;

            var table = new DataTable();
            await connection.OpenAsync();
            adapter.Fill(table);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapRowToDto(table);
        }

        public async Task<bool> ExistsAsync(Guid empId)
        {
            const string query = "SELECT COUNT(1) FROM dbo.Employees WHERE EmpID = @EmpID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@EmpID", empId);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<bool> UpdateAsync(Guid empId, EmployeeDetailsDto employeeDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var personPK = await GetPersonPKAsync(empId, connection, transaction);
                if (personPK == null)
                {
                    return false;
                }

                await UpdatePersonAsync(personPK.Value, employeeDto, connection, transaction);
                await UpdateEmployeeAsync(empId, employeeDto, connection, transaction);
                await UpdateUserAsync(personPK.Value, employeeDto, connection, transaction);
                await UpdatePhoneNumbersAsync(personPK.Value, employeeDto.PhoneNumbers, connection, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private static EmployeeDetailsDto MapRowToDto(DataTable table)
        {
            var row = table.Rows[0];
            var phoneNumbers = new List<string>();
            var apiNames = new List<string>();
            var benefitNames = new List<string>();

            foreach (DataRow dataRow in table.Rows)
            {
                AddUniqueStringToList(dataRow["PhoneNumber"].ToString(), phoneNumbers);
                AddUniqueStringToList(dataRow["ApiName"].ToString(), apiNames);
                AddUniqueStringToList(dataRow["BenefitName"].ToString(), benefitNames);
            }

            return new EmployeeDetailsDto
            {
                EmpID = row["EmpID"].ToString(),
                Id = row["Id"]?.ToString(),
                FirstName = row["FirstName"].ToString() ?? string.Empty,
                LastName = row["LastName"].ToString() ?? string.Empty,
                Sex = row["Sex"]?.ToString(),
                BirthDate = Convert.ToDateTime(row["BirthDate"]),
                Province = row["Province"]?.ToString(),
                Canton = row["Canton"]?.ToString(),
                OtherSigns = row["OtherSigns"]?.ToString(),
                JobPosition = row["JobPosition"]?.ToString(),
                ContractType = row["ContractType"]?.ToString(),
                WorkHours = row["WorkHours"] != DBNull.Value ? Convert.ToInt32(row["WorkHours"]) : null,
                StartDate = Convert.ToDateTime(row["StartDate"]),
                BankAccount = row["BankAccount"]?.ToString(),
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

        private static void AddUniqueStringToList(string? value, List<string> list)
        {
            if (!string.IsNullOrEmpty(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }

        private static async Task<Guid?> GetPersonPKAsync(Guid empId, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = "SELECT PersonPK FROM dbo.Employees WHERE EmpID = @EmpID";

            using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@EmpID", empId);

            var result = await command.ExecuteScalarAsync();
            return result as Guid?;
        }

        private static async Task UpdatePersonAsync(Guid personPK, EmployeeDetailsDto dto, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
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

            using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@Id", dto.Id ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FirstName", dto.FirstName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LastName", dto.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Sex", dto.Sex ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Province", dto.Province ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Canton", dto.Canton ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@OtherSigns", dto.OtherSigns ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PersonPK", personPK);

            await command.ExecuteNonQueryAsync();
        }

        private static async Task UpdateEmployeeAsync(Guid empId, EmployeeDetailsDto dto, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
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

            using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@JobPosition", dto.JobPosition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ContractType", dto.ContractType ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@WorkHours", dto.WorkHours ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@BankAccount", dto.BankAccount ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@GrossSalary", dto.GrossSalary);
            command.Parameters.AddWithValue("@PayCycle", dto.PayCycle ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RegistersHours", dto.RegistersHours);
            command.Parameters.AddWithValue("@EmpID", empId);

            await command.ExecuteNonQueryAsync();
        }

        private static async Task UpdateUserAsync(Guid personPK, EmployeeDetailsDto dto, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
                UPDATE dbo.Users 
                SET 
                    Email = @Email,
                    Role = @Role,
                    Active = @Status
                WHERE PersonPK = @PersonPK";

            using var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@Email", dto.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Role", dto.Role ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Status", dto.Status);
            command.Parameters.AddWithValue("@PersonPK", personPK);

            await command.ExecuteNonQueryAsync();
        }

        private static async Task UpdatePhoneNumbersAsync(Guid personPK, List<string> phoneNumbers, SqlConnection connection, SqlTransaction transaction)
        {
            // Delete existing phone numbers
            const string deleteQuery = "DELETE FROM dbo.PersonPhoneNumbers WHERE PersonPK = @PersonPK";
            using var deleteCommand = new SqlCommand(deleteQuery, connection, transaction);
            deleteCommand.Parameters.AddWithValue("@PersonPK", personPK);
            await deleteCommand.ExecuteNonQueryAsync();

            // Insert new phone numbers
            if (phoneNumbers?.Any() == true)
            {
                const string insertQuery = "INSERT INTO dbo.PersonPhoneNumbers (PersonPK, Number) VALUES (@PersonPK, @Number)";

                foreach (var phoneNumber in phoneNumbers.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    using var insertCommand = new SqlCommand(insertQuery, connection, transaction);
                    insertCommand.Parameters.AddWithValue("@PersonPK", personPK);
                    insertCommand.Parameters.AddWithValue("@Number", phoneNumber.Trim());
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }
    }
}