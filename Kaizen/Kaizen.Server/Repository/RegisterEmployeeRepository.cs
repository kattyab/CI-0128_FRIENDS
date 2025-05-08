using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Kaizen.Server.Models;

namespace Kaizen.Server.Repository
{

    public class RegisterEmployeeRepository
    {
        private readonly string _connectionString;

        public RegisterEmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KaizenDb")
                ?? throw new InvalidOperationException(
                    "La cadena de conexion 'KaizenDb' no está definida en appsettings.json");
        }

        public async Task<bool> CreateEmployee(RegisterEmployeeDto employee)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    string getCompanySql = employee.Adminrole switch
                    {
                        "Administrador" => @"
                    SELECT A.CompanyPK
                    FROM Admins A
                    JOIN Users U ON A.AdminPK = U.PersonPK
                    WHERE U.Email = @AdminEmail;",
                        "Dueño" => @"
                    SELECT C.CompanyPK
                    FROM Companies C
                    JOIN Persons P ON C.OwnerPK = P.PersonPK
                    JOIN Users U ON P.PersonPK= U.PersonPK
                    WHERE U.Email = @AdminEmail;",
                        _ => throw new Exception("Invalid role for retrieving company.")
                    };

                    string companyPK;
                    using (SqlCommand companyCmd = new SqlCommand(getCompanySql, conn))
                    {
                        companyCmd.Parameters.AddWithValue("@AdminEmail", employee.Adminemail);
                        var result = await companyCmd.ExecuteScalarAsync()
                            ?? throw new Exception("Admin email not found or not associated with a company.");
                        companyPK = result.ToString();
                    }

                    var hasher = new PasswordHasher<string>();
                    string hashedPassword = hasher.HashPassword(employee.Email, employee.Password);

                    DateTime birthdate = DateTime.Parse(employee.Birthdate);
                    DateTime startdate = DateTime.Parse(employee.Startdate);
                    Guid personPK = Guid.NewGuid();

                    string insertSql = @"
INSERT INTO Persons (PersonPK, Id, Name, LastName, Sex, BirthDate, Province, Canton, OtherSigns)
VALUES (@PersonPK, @Id, @Name, @LastName, @Sex, @BirthDate, @Province, @Canton, @OtherSigns);

INSERT INTO Users (Email, PasswordHash, Active, Role, PersonPK)
VALUES (@Email, @PasswordHash, 1, @Role, @PersonPK);

INSERT INTO PersonPhoneNumbers (PersonPK, Number)
VALUES (@PersonPK, @PhoneNumber);

INSERT INTO Employees (PersonPK, WorksFor, JobPosition, ContractType, WorkHours, ExtraHours, StartDate, BankAccount, BruteSalary, PayCycleType)
VALUES (@PersonPK, @CompanyPK, @JobPosition, @ContractType, 0, 0, @StartDate, @BankAccount, @BruteSalary, @PayCycleType);
";

                    if (employee.Role == "Administrador")
                    {
                        insertSql += @"
INSERT INTO Admins (AdminPK, CompanyPK)
VALUES (@PersonPK, @CompanyPK);";
                    }

                    using SqlCommand cmd = new SqlCommand(insertSql, conn);
                    cmd.Parameters.AddWithValue("@PersonPK", personPK);
                    cmd.Parameters.AddWithValue("@Id", employee.Personid);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@LastName", employee.Lastname);
                    cmd.Parameters.AddWithValue("@Sex", employee.Sex);
                    cmd.Parameters.AddWithValue("@BirthDate", birthdate);
                    cmd.Parameters.AddWithValue("@Province", employee.Province);
                    cmd.Parameters.AddWithValue("@Canton", employee.Canton);
                    cmd.Parameters.AddWithValue("@OtherSigns", employee.Othersigns);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Role", employee.Role);
                    cmd.Parameters.AddWithValue("@PhoneNumber", employee.Phonenumber);
                    cmd.Parameters.AddWithValue("@CompanyPK", companyPK);
                    cmd.Parameters.AddWithValue("@JobPosition", employee.Jobposition);
                    cmd.Parameters.AddWithValue("@ContractType", employee.Contract);
                    cmd.Parameters.AddWithValue("@StartDate", startdate);
                    cmd.Parameters.AddWithValue("@BankAccount", employee.Bankaccount);
                    cmd.Parameters.AddWithValue("@BruteSalary", employee.Brutesalary);
                    cmd.Parameters.AddWithValue("@PayCycleType", employee.Paycycle);

                    await cmd.ExecuteNonQueryAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error inserting employee data", ex);
                }
            }
        }

    }
}